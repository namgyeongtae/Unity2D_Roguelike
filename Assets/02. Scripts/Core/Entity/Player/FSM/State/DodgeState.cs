using UnityEngine;

public class DodgeState : State<Entity>
{
    private PlayerController _playerController;

    protected override void Setup()
    {
        _playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        Entity.SocketPivot.gameObject.SetActive(false);
        SetAnimation();

        // 무적 상태임
    }

    public override void Exit()
    {
        // 무적 상태 해제
    }

    void SetAnimation() => Entity.PlayAnimation($"Dodge_{_playerController.Direction}");
}
