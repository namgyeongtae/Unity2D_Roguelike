using UnityEngine;

public class IdleState : State<Entity>
{
    private PlayerController playerController;

    protected override void Setup()
    {
        playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        SetAnimation();
    }

    void SetAnimation() => Entity.PlayAnimation($"Idle_{playerController.Direction}");
}
