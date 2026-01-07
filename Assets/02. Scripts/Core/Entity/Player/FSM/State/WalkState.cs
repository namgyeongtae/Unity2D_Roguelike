using UnityEngine;

public class WalkState : State<Entity>
{
    private PlayerController playerController;

    protected override void Setup()
    {
        playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        Debug.Log("WalkState Enter");
        // SetAnimation();
    }

    public override void Update()
    {
        SetAnimation();
    }

    void SetAnimation() => Entity.PlayAnimation($"Walk_{playerController.Direction}");
}
