using UnityEngine;

public class DodgeState : State<Entity>
{
    private PlayerController _playerController;

    protected override void Setup()
    {

    }

    public override void Enter()
    {
        _playerController.State = EntityState.Dodge;

        Entity.SocketPivot.gameObject.SetActive(false);
    }
}
