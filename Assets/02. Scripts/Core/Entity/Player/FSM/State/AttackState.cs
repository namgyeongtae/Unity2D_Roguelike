using UnityEngine;

public class AttackState : State<Entity>
{
    private PlayerController playerController;

    protected override void Setup()
    {
        playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        Debug.Log("AttackState Enter");
    }

    public override void Exit()
    {
        
    }
}
