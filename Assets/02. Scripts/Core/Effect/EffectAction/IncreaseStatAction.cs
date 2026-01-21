using UnityEngine;

[System.Serializable]
public class IncreaseStatAction : EffectAction
{
    [SerializeField] private Stat stat;     // 증가/감소 시킬 Stat (힘이 오면 STR, 체력이 오면 HP etc...)
    [SerializeField] private float defaultValue;    // 수치를 얼마나 증가시킬 것인가?
    [SerializeField] private Stat bonusValueStat;   
    [SerializeField] private float bonusValueStatFactor;
    [SerializeField] private float bonusValuePerLevel;
    [SerializeField] private float bonusValuePerStack;

    // 적용할 값을 Stat의 DefaultValue에 더할 것인가? Bonus Value로 추가할 것인가?
    [SerializeField] private bool isBonusType = true;
    // 적용한 값을 Release할 때 되돌릴 것인가?
    [SerializeField] private bool isUndoOnRelease = true;

    private float totalValue;   // Stat을 증가시킨 수치를 저장하기 위한 변수, Release할 때 이 수치를 빼주는 걸로 Undo 실행행

    private float GetDefaultValue(Effect effect)
        => defaultValue + (effect.DataBonusLevel * bonusValuePerLevel);

    private float GetStackValue(int stack)
        => (stack - 1) * bonusValuePerStack;

    private float GetBonusStatValue(Entity user)
        => user.Stats.GetValue(bonusValueStat) * bonusValueStatFactor;

    private float GetTotalValue(Effect effect, Entity user, int stack, float scale)
    {
        totalValue = GetDefaultValue(effect) + GetStackValue(stack);
        if (bonusValueStat)
            totalValue += GetBonusStatValue(user);

        totalValue *= scale;

        return totalValue;
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        totalValue = GetTotalValue(effect, user, stack, scale);

        Debug.Log($"Increase {stat.DisplayName} : {totalValue}");

        if (isBonusType)
            target.Stats.SetBonusValue(stat, this, totalValue);
        else
            target.Stats.IncreaseDefaultValue(stat, totalValue);

        return true;
    }

    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        if (!isUndoOnRelease)
            return;

        if (isBonusType)
            target.Stats.RemoveBonusValue(stat, this);
        else
            target.Stats.IncreaseDefaultValue(stat, -totalValue);
    }

    public override void OnEffectStackChanged(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        if (!isBonusType)
            Release(effect, user, target, level, scale);    // isBonusType이 아니라면 싹 지우고

        Apply(effect, user, target, level, stack, scale);   // 현재 stack만큼 다시 적용
    }

    public override object Clone()
    {
        return new IncreaseStatAction()
        {
            stat = stat,
            defaultValue = defaultValue,
            bonusValueStat = bonusValueStat,
            bonusValueStatFactor = bonusValueStatFactor,
            bonusValuePerLevel = bonusValuePerLevel,
            bonusValuePerStack = bonusValuePerStack,
            isBonusType = isBonusType,
            isUndoOnRelease = isUndoOnRelease,
        };
    }
}
