using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class EffectStackAction
{
    // 이 Action이 Effect의 Stack이 몇일 때 적용될 것인가?
    [SerializeField, Min(1)] private int stack;
    // Effect가 다음 StackAction을 적용할 때 이 Action을 Release 할 것인가?
    [SerializeField] private bool isReleaseOnNextApply;

    // Effect 마다 1번씩만 적용될 것인가?
    // ex. 이 Option이 켜져있으면, Action의 필요한 Stack이 2일 때 Effect의 Stack이 2가 되면 Action이 적용되고,
    // Effect의 Stack이 3으로 증가되었다가 다시 2로 떨어져도, Action이 이미 한 번 적용되었기 때문에 다시 적용되지 않음
    [SerializeField] private bool isApplyOnceInLifeTime;

    [UnderlineTitle("Action")]
    [SerializeReference, SubclassSelector]
    private EffectAction action;

    // 이 StackAction이 적용된 적이 있는가?
    private bool hasEverApplied;

    public int Stack => stack;
    public bool IsReleaseOnNextApply => isReleaseOnNextApply;

    // isApplyOnceInLifeTime이 true라면, 적용된 적이 없어야 적용 가능함
    // 헷갈리는 거 -> isApplyOnceInLifeTime은 이미 적용된 적 있는지 없는지에 대한 여부가 아니라,
    // 그냥 한 번만 적용하게 할 건지 아닌지에 대한 여부임
    // 따라서 아래 조건은 
    // 한 번만 적용하는 게 아니"거나"
    // 한 번만 적용하는데 한 번도 적용된 적 없으면 true임
    public bool IsApplicable => !isApplyOnceInLifeTime || (isApplyOnceInLifeTime && !hasEverApplied);

    public void Start(Effect effect, Entity user, Entity target, int level, float scale)
        => action.Start(effect, user, target, level, scale);
    
    public void Apply(Effect effect, int level, Entity user, Entity target, float scale)
    {
        action.Apply(effect, user, target, level, stack, scale);
        hasEverApplied = true;
    }

    public void Release(Effect effect, int level, Entity user, Entity target, float scale)
        => action.Release(effect, user, target, level, scale);

    public string BuildDescription(Effect effect, string baseDescription, int stackActionIndex, int effectIndex)
        => action.BuildDescription(effect, baseDescription, stackActionIndex, stack, effectIndex);
}
