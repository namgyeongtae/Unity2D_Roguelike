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
    }

    void SetAnimation() => Entity.PlayAnimation($"Dodge_{_playerController.Direction}");
}
