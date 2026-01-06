using UnityEngine;

public class LeftState : State<Entity>
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
        _playerController.Direction = EntityDirection.Left;
    }
    
    public override void Exit()
    {
        
    }
}
