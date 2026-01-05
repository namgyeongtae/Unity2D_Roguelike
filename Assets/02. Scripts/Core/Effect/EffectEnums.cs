public enum EffectType
{
    None,
    Buff,
    Debuff
}

public enum EffectRemoveDuplicateTargetOption
{
    Old, // 이미 적용되어 있는 효과 제거
    New // 새로 적용되는 효과 제거
}

public enum EffectRunningFinishOption
{
    // Effect가 설정된 적용 횟수만큼 적용된다면 완료되는 Option
    // 단, 이 Option은 지속 시간(= Duration)이 끝나도 완료됨
    // 타격을 입히거나, 치료를 해주는 Effect에 적합
    FinishWhenApplyCompleted,

    // 지속 시간이 끝나면 완료되는 Option
    // Effect가 설정된 적용 횟수만큼 적용되도, 지속 시간이 남았다면 완료가 안됨.
    // 처음 한 번 적용되고, 일정 시간동안 지속되는 Buff나 Debuff Effect에 적합한 Option
    FinishWhenDurationEnded
}