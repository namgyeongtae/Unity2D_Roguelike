using UnityEngine;

public class RightState : State<Entity>
{
    private SpriteRenderer _spriteRenderer;
    private PlayerController _playerController;

    protected override void Setup()
    {
        _spriteRenderer = Entity.GetComponentInChildren<SpriteRenderer>();
        _playerController = Entity.GetComponent<PlayerController>();
    }

    public override void Enter()
    {
        _playerController.Direction = EntityDirection.Right;
    }
    
    public override void Exit()
    {
        
    }
}
