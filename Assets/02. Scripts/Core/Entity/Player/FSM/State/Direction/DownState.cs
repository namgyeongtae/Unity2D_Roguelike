using UnityEngine;

public class DownState : State<Entity>
{
    private PlayerController _playerController;
    protected override void Setup()
    {
        _playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
       
    }

    public override void Update()
    {
        
    }

    public override void Exit()
    {

    }
}
