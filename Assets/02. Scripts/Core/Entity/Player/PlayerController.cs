using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Entity entity;

    private bool isAttacking = false;
    private bool isHit = false;

    public bool IsAttacking => isAttacking;

    public bool CanMove => !isAttacking && !isHit;
    public bool CanAttack => !isHit;

    void Start()
    {
        entity = GetComponent<Entity>();

        InstallKeyBindings();
    }

    void OnEnable()
    {
        /* KeyInputController.Instance.onKeyInputDownDirection += Move;
        KeyInputController.Instance.onKeyInputUpDirection += Stop; */

        KeyInputController.Instance.onDirectionInput += Move;
    }

    void OnDisable()
    {
        /* KeyInputController.Instance.onKeyInputDownDirection -= Move;
        KeyInputController.Instance.onKeyInputUpDirection -= Stop; */
        KeyInputController.Instance.onDirectionInput -= Move;
    }

    void InstallKeyBindings()
    {
        KeyInputController.Instance.SetCommand(ActionId.Attack, InputPhase.Down, Attack);
    }
    
    void Move(Vector2 dir)
    {
        if (!CanMove)
            return;

        entity.Movement?.Move(dir);
    }

    void Attack()
    {
        isAttacking = true;
    }

    public void AttackEnd()
    {
        isAttacking = false;
    }
}
