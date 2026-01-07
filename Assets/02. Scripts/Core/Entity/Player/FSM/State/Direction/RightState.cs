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
       
    }

    public override void Update()
    {
        
    }
    
    public override void Exit()
    {
        
    }
}
