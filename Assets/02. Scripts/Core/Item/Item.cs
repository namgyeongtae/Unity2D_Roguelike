using UnityEngine;

public class Item : IdentifiedObject
{
    public delegate void UseItemHandler(Item item, Entity user); // 소모 아이템의 경우 인벤토리에서 제거한다던가 그런 Action 트리거거

    // 장비아이템인가? 소비아이템인가?
    [SerializeField] private ItemType type;
    [SerializeField] private ItemEquipmentSlot equipmentSlot;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private bool isNeedTarget;
    [SerializeField] private ItemTargetType targetType;

    public Entity User { get; private set; }

    public event UseItemHandler onUsed;


    public ItemType GetItemType() => type;

    public Effect effect;

    public bool TryUseItem(Entity user)
    {
        if (type != ItemType.Equipment && type != ItemType.Consumable)
        {
            Debug.LogError("Item::Use - 장비 아이템이나 소모 아이템이 아닙니다.");
            return false;
        }

        bool isSuccess = false;

        if (isNeedTarget)
        {
            var realTarget = targetType == ItemTargetType.Self ? user : null;

            Debug.Assert(realTarget != null, "Item::Use - Target은 null일 수 없습니다.");

            if (realTarget == user)
                isSuccess = ApplyEffect(user, realTarget);
            else
            {
                // Target은 고르는 Action이 실행되야 함
                // 잠시 Target을 찾는 Find 모드로 빠져서 마우스 클릭하면 해당 위치의 Entity를 Target으로 하고
                // ApplyEffect(user, 클릭한 Entity) 실행하도록록
            }
        }

        onUsed?.Invoke(this, user);
        return isSuccess;
    }

    public void Setup(Entity user)
    {
        User = user;
    }

    private bool ApplyEffect(Entity user, Entity target)
    {
        var effectManager = target.GetComponent<EntityEffectManager>();
        if (effectManager.HasEffect(effect))
        {
            Debug.LogError("Item::ApplyEffect - 이미 적용된 Effect입니다.");
            return false;
        }

        var cloneEffect = effect.Clone() as Effect;
        cloneEffect.Setup(this, user, 1, 1f);
        cloneEffect.SetTarget(target);

        target.GetComponent<EntityEffectManager>().AddEffect(cloneEffect);
        return true;
    }
}
