using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect : IdentifiedObject
{
    #region Variables
    private const int kInfinity = 0;

    #region Delegate

    public delegate void StartedHandler(Effect effect);
    public delegate void AppliedHandler(Effect effect, int currentApplyCount, int prevApplyCount);
    public delegate void ReleasedHandler(Effect effect);
    public delegate void StackChangedHandler(Effect effect, int currentApplyCount, int prevApplyCount);

    #endregion

    [SerializeField] private EffectType type;

    [SerializeField] private bool isAllowDuplicate = true;
    [SerializeField] EffectRemoveDuplicateTargetOption removeDuplicateTargetOperation;

    [SerializeField] private bool isShowInUI;

    // maxLevel이 effectDatas의 Length를 초과할 수 있는지 여부
    // 이 Option이 false면 maxLevel은 effectDatas의 Length로 고정됨
    [SerializeField] private bool isAllowLevelExceeedDatas;
    [SerializeField] private int maxLevel;

    // Level 별 Data, Level은 1부터 시작하고 Array의 Index는 0부터 시작하므로
    // Level에 맞는 Data를 가져오려면 [현재 Level - 1]번째 Data를 가져와야 함
    // ex. Level이 1이라면, 1 - 1 = 0, 0번째 Data를 가져와야함(= effectDatas[0])
    [SerializeField] private EffectData[] effectDatas;

    // Level에 맞는 현재 Data
    private EffectData currentData;

    // 현재 Effect의 Level
    private int level;
    // 현재 쌓인 Stack
    private int currentStack = 1;
    private float currentDuration; // 현재 Effect의 지속 시간
    private int currentApplyCount; // 현재 Effect의 적용 횟수
    private float currentApplyCycle; // 현재 Effect의 적용 주기

    // Action의 Apply 함수를 실행하려 시도한 적이 있는지 여부, 이 값에 따라 Apply 성공 시에 currentApplyCycle 변수의 값을 다르게 초기화 함
    // Action의 Apply 함수가 실행될 때 true가 되고, Apply 함수가 true를 return 하면 false로 초기화 됨
    private bool isApplyTried;

    // 쌓인 Stack에 따라 현재 적용된 Stack Actions
    private readonly List<EffectStackAction> appliedStackActions = new();

    public EffectType Type => type;
    public bool IsAllowDuplicate => isAllowDuplicate;
    public EffectRemoveDuplicateTargetOption RemoveDuplicateTargetOperation => removeDuplicateTargetOperation;

    public bool IsShowInUI => isShowInUI;

    public IReadOnlyList<EffectData> EffectDatas => effectDatas;
    public IReadOnlyList<EffectStackAction> StackActions => currentData.stackActions;

    public int MaxLevel => maxLevel;

    public int Level
    {
        get => level;
        set
        {
            Debug.Assert(value > 0 && value <= maxLevel, $"Effect.Rank = {value} - value는 0보다 크고 MaxLevel 보다 작거나 같아야 합니다.");

            if (level == value)
                return;

            var newData = effectDatas.Last(x => x.level <= value);
            if (currentData == null || newData.level != currentData.level)
                currentData = newData;
        }
    }

    public bool IsMaxLevel => level == maxLevel;

    // 현재 Effect와 EffectData의 Level 차이
    // Action 쪽에서 Bonus Value를 주는데 활용할 수 있음
    // ex. totalValue = defaultValue + (effect.DataBonusLevel * bonusValuePerLevel)
    // Level이 1~1000까지 있는 Clicker Game의 경우 Data를 1000까지 만들지 않아도
    // 위와 같이 Bonus Level을 활용해 Level 당 수치를 조절할 수 있음.
    public int DataBonusLevel => Mathf.Max(level - currentData.level, 0);


    public float Duration => currentData.duration.GetValue(User.Stats);

    public bool IsTimeless => Mathf.Approximately(Duration, kInfinity);

    public float CurrentDuration
    {
        get => currentDuration;
        set => currentDuration = Mathf.Clamp(value, 0f, Duration);
    }

    public float RemainDuration => Mathf.Max(0f, Duration - currentDuration);

    public int MaxStack => currentData.maxStack;

    public int CurrentStack
    {
        get => currentStack;
        set
        {
            var prevStack = currentStack;
            currentStack = Mathf.Clamp(value, 1, MaxStack);

            // Stack이 쌓이면 currentDuration을 초기화하여 Effect의 지속 시간을 늘려줌
            // 꼭 >= 이어야 한다.
            // > 로만 currentDuration을 초기화하면
            // 최대 스택까지 쌓인 상황에서 현재 지속 시간이 초기화가 되지 않게 된다.
            // Ex) 장판 위에 있으면 받는 효과의 경우 최대 스택에서 Duration이 지난 후 효과가 해제되고 바로 다시 적용되는 이상한 상황이 발생하기 때문
            if (currentStack >= prevStack)
                currentDuration = 0f;

            if (currentStack != prevStack)
            {
                // Action에 쌓인 Stack 수가 바뀌었다고 알려줘서, Stack에 따른 수치를 Update 할 수 있게 함
                Action?.OnEffectStackChanged(this, User, Target, Level, currentStack, Scale);

                TryApplyStackActions();
                
                onStackChanged?.Invoke(this, currentStack, prevStack);
            }
        }
    }

    public int ApplyCount => currentData.applyCount;

    public bool IsInfinitelyApplicable => ApplyCount == kInfinity;

    public int CurrentApplyCount
    {
        get => currentApplyCount;
        set => currentApplyCount = IsInfinitelyApplicable ? value : Mathf.Clamp(value, 0, ApplyCount);
    }

    // ApplyCycle이 0이고 ApplyCount가 1보다 크면 Effect는 지속 시간인 Duration을 나눠서 ApplyCycle을 계산함
    // 예를 들어 Duration이 10초고 ApplyCount 11번이면, 처음 Effect가 적용될 때 Apply가 1번 이뤄져서
    // 남은 ApplyCount = 10, Duration / ApplyCount = 10 / 10 = 1, ApplyCycle = 1
    public float ApplyCycle => Mathf.Approximately(currentData.applyCycle, 0f) && ApplyCount > 1 ? 
        (Duration / (ApplyCount - 1)) : currentData.applyCycle;
    
    // ApplyCycle을 확인하기 위한 변수.
    // CurrentDuration을 이용해서 확인하지 않고 CurrentApplyCycle을 따로 만든 이유는
    // CurrentDuration은 Effect의 Stack이 쌓이면 0으로 초기화되기 때문.
    // 예를 들어, ApplyCycle이 1초이고 CurrentDuration이 0.9999초 일 때,
    // 원래는 다음 Frame에 CurrentDuration이 1초가 되면서 Effect가 Apply되야 하는데,
    // Stack이 쌓여서 CurrentDuration이 0초로 초기화 되버리면, 1초를 다시 기다려야 Apply 되는 상황이 옴
    // 이런 상황이 계속 반복되면 영원히 Effect는 Apply 되지 않게 된다.
    // 그래서 따로 Apply 시점 확인하는 CurrentApplyCycle이 있으면 CurrentDuration이 중간에 0이 되도
    // CurrentApplyCycle은 계속 시간이 쌓이고 있으니 제때 Apply 될 수 있게 해줌
    public float CurrentApplyCycle 
    {
        get => currentApplyCycle;
        set => currentApplyCycle = Mathf.Clamp(value, 0f, ApplyCycle);
    }


    private EffectAction Action => currentData.action;
    private CustomAction[] CustomActions => currentData.customActions;

    public object Owner { get; private set; }
    public Entity User { get; private set; }
    public Entity Target { get; private set; }

    // Scale 조절을 통해 Effect의 위력을 조절할 수 있음
    // Charge처럼 Casting 시간에 따라 위력이 달라지는 Skill에 활용될 수 있음
    public float Scale { get; set; }
    public override string Description => BuildDescription(base.Description, 0);


    private bool IsApplyAllWhenDurationExpires => currentData.isApplyAllWhenDurationExpires;
    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount == ApplyCount;

    // Effect 완료 여부
    // 지속 시간이 끝났거나, RunningFinishOption이 ApplyCompleted일 때, Apply 횟수가 최대 횟수라면 True
    public bool IsFinished => IsDurationEnded ||
        (currentData.runningFinishOption == EffectRunningFinishOption.FinishWhenApplyCompleted && IsApplyCompleted);

    // Effect의 Release 함수가 실행되면 (= Effect가 종료되면) True가 됨
    // IsFinished Property가 Effect가 온전히 종료되어야만 True인 반면, IsReleased는 무언가에 의해 Effect가 제거되어도 True가 됨
    // 완료 여부와 상관없이 순수히 Effect가 종료되었는지 확인하기 위한 Property
    public bool IsReleased { get; private set; }

    // Effect를 적용할 수 있는가?
    // Action이 존재하고 
    // CurrentApplyCount가 ApplyCount보다 작거나, ApplyCount가 무한이고 (= 최대 적용 횟수를 넘기지 않았다면)
    // CurrentApplyCycle이 ApplyCycle보다 크거나 같으면(= 적용 주기를 다 채웠다면)
    // True
    public bool IsApplicable => Action != null 
        && (CurrentApplyCount < ApplyCount || ApplyCount == kInfinity) 
        && (CurrentApplyCycle >= ApplyCycle);

    public event StartedHandler onStarted;
    public event AppliedHandler onApplied;
    public event ReleasedHandler onReleased;
    public event StackChangedHandler onStackChanged;

    #endregion

    #region Methods

    public void Setup(object owner, Entity user, int level, float scale = 1f)
    {
        Owner = owner;
        User = user;
        Level = level;
        CurrentApplyCycle = ApplyCycle;
        Scale = scale;
    }

    public void SetTarget(Entity target) => Target = target;

    // 현재 적용된 모든 StackAction들을 Release 함
    private void ReleaseStackActionAll()
    {
        appliedStackActions.ForEach(x => x.Release(this, Level, User, Target, Scale));
        appliedStackActions.Clear();
    }

    // 현재 적용된 StackAction 들에서 조건에 맞는 StackAction들을 찾아 Release 함
    private void ReleaseStackActions(Func<EffectStackAction, bool> predicate)
    {
        var stackActions = appliedStackActions.Where(predicate).ToList();
        foreach (var stackAction in stackActions)
        {
            stackAction.Release(this, Level, User, Target, Scale);
            appliedStackActions.Remove(stackAction);
        }
    }

    // 현재 적용된 StackAction들 중 더 이상 조건에 맞지 않는 StackAction들은 Release하고,
    // 새롭게 조건에 맞는 StackAction들을 적용하는 함수
    private void TryApplyStackActions()
    {
        // 적용된 StackAction들 중 현재 Stack보다 더 큰 Stack을 요구하는 StackAction들을 Release 함
        // 어떤 이유에 의해 Stack 수가 떨어졌을 때를 위한 처리.
        ReleaseStackActions(x => x.Stack > CurrentStack);

        // 적용 가능한 StackAction 목록
        // StackAction들 중에서 필요한 Stack 수가 충족되고, 현재 적용 중이지 않고, 적용 조건을 만족하는 StackAction들을 찾아옴
        var stackActions = StackActions.Where(x => x.Stack <= currentStack && !appliedStackActions.Contains(x) && x.IsApplicable);

        // 현재 적용된 StackAction들과 찾아온 StackAction들 중 가장 높은 Stack값을 찾아옴
        int appliedStackHighestStack = appliedStackActions.Any() ? appliedStackActions.Max(x => x.Stack) : 0;
        int stackActionsHighestStack = stackActions.Any() ? stackActions.Max(x => x.Stack) : 0;
        var highestStack = Mathf.Max(appliedStackHighestStack, stackActionsHighestStack);
        if (highestStack > 0)
        {
            // 찾아온 StackAction들 중 Stack이 highestStack보다 낮고, IsReleaseOnNextApply가 true인 StackAction들을 찾아옴
            var except = stackActions.Where(x => x.Stack < highestStack && x.IsReleaseOnNextApply);

            // 바로 위에서 찾아온 stackAction들을 stackActions 목록에서 제외함
            // => IsReleaseOnNextApply가 true인 StackAction은 더 높은 Stack을 가진 StackAction이 존재한다면
            // Release되야 하므로 애초에 적용 대상 목록에서 제거함
            stackActions = stackActions.Except(except);
        }

        if (stackActions.Any())
        {
            // 적용된 StackAction들 중에서 IsReleaseOnNextApply가 true인 StackAction들을 Release 함
            // 단, 필요 Stack이 현재 Stack과 동일한 StackAction 들은 제외.
            // 왜냐하면 예를 들어, Stack 수가 5 필요한 StackAction이 적용 중이고, 현재 Effect Stack이 6이였다가 5로 떨어졌을 경우
            // 적용 중인 StackAction의 필요 Stack 수 5와 현재 Efect Stack 5가 일치하므로 가만 놔두면 되는데,
            // x.Stack < currentStack라는 조건이 없으면, 현재 Effect Stack 수와 일치하는 StackAction들 까지 Release의 대상으로 포함됨
            ReleaseStackActions(x => x.Stack < currentStack && x.IsReleaseOnNextApply);

            foreach (var stackAction in stackActions)
                stackAction.Apply(this, level, User, Target, Scale);
            
            appliedStackActions.AddRange(stackActions);
        }
    }

    public void Start()
    {
        Debug.Assert(!IsReleased, "Effect::Start - 이미 종료된 Effect 입니다.");

        Action?.Start(this, User, Target, Level, Scale);

        TryApplyStackActions();

        foreach (var customAction in CustomActions)
            customAction.Start(this);
        
        onStarted?.Invoke(this);
    }

    public void Update()
    {
        CurrentDuration += Time.deltaTime;
        CurrentApplyCycle += Time.deltaTime;

        if (IsApplicable)
            Apply();

        if (IsApplyAllWhenDurationExpires && IsDurationEnded && !IsInfinitelyApplicable)
        {
            for (int i = currentApplyCount; i < ApplyCount; i++)
                Apply();
        }
    }

    public void Apply()
    {
        Debug.Assert(!IsReleased, "Effect::Apply - 이미 종료된 Effect 입니다.");

        if (Action == null)
            return;

        if (Action.Apply(this, User, Target, level, currentStack, Scale))
        {
            foreach (var customAction in CustomActions)
                customAction.Run(this);
            
            var prevApplyCount = currentApplyCount++;

            // Time Sync 문제를 해결하기 위해 if-else를 나누었다.
            // ex. Skill의 Duration이 1초이고 ApplyCycle이 0.5초고 ApplyCount가 3회라면
            // 처음에 한 번 적용되고 0.5초 뒤에 한 번 적용되고 다시 0.5초 뒤에 한 번 적용될 것이다. 
            // 그런데 Update함수를 보면 현재 Duraiton과 현재 ApplyCycle에 Time.deltaTime를 더해주는데,
            // deltaTime을 계속 더하다가 두 값이 Apply Timing인 0.5초에 도달했다고 가정해보자.
            // 이 둘의 값이 정확이 0.5일 것인가? 아니다. 약간의 오차값이 존재할 것이다.
            // 그러면 이 오차가 존재하는 상태에서 효과 적용 후 현재 ApplyCycle을 0으로 초기화 했다고 가정해보자.
            // 그러면 Duration은 0.5xxx, 현재 ApplyCycle은 0이고, 0.5를 기준으로 했을 때
            // 둘의 오차가 0.0xxx초인 상황이다.
            // 이 상태로 deltaTime이 계속 더해지다가 Duration이 1초가 됬다면 
            // 이제 Effect를 끝내야 하는데 ApplyCycle은 오차 때문에 0.5초가 못되고 오차 시간만큼 부족하게 된다.
            // 그럼 Apply를 못하게 되는데 Effect를 끝내야 하는 상황이 온 것이다.
            // 즉, 오차값 대문에 목표인 3번을 적용 못하고 2번만 적용하고 끝내야 한다.
            // IsApplyAllWhenDurationExpires 가 true면 상관 없지만 이는 근본적인 해결책은 못된다.
            if (isApplyTried)
                currentApplyCycle = 0f;
            else 
                currentApplyCycle %= ApplyCycle;

            isApplyTried = false;

            onApplied?.Invoke(this, currentApplyCount, prevApplyCount);
        }
        else
            isApplyTried = true;
    }

    public void Release()
    {
        Debug.Assert(!IsReleased, "Effect::Release - 이미 종료된 Effect 입니다.");

        Action?.Release(this, User, Target, level, Scale);
        ReleaseStackActionAll();

        foreach (var customAction in CustomActions)
            customAction.Release(this);
        
        IsReleased = true;

        onReleased?.Invoke(this);
    }

    public EffectData GetData(int level) => effectDatas[level - 1];

    public string BuildDescription(string description, int effectIndex)
    {
        Dictionary<string, string> stringsByKeyword = new Dictionary<string, string>()
        {
            { "duration", Duration.ToString(".##") },
            { "applyCount", ApplyCount.ToString() },
            { "applyCycle", ApplyCycle.ToString(".##") },
        };

        description = TextReplacer.Replace(description, stringsByKeyword, effectIndex.ToString());

        description = Action.BuildDescription(this, description, 0, 0, effectIndex);

        var stackGroups = StackActions.GroupBy(x => x.Stack);
        foreach (var stackGroup in stackGroups)
        {
            int i = 0;
            foreach (var stackAction in stackGroup)
                description = stackAction.BuildDescription(this, description, i++, effectIndex);
        }

        return description;
    }

    public override object Clone()
    {
        var clone = Instantiate(this);

        if (Owner != null)
            clone.Setup(Owner, User, Level, Scale);
        
        return clone;
    }

    #endregion

}