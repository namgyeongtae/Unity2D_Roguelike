using UnityEngine;

public class LeftState : State<Entity>
{
    private SpriteRenderer _spriteRenderer;

    protected override void Setup()
    {
        _spriteRenderer = Entity.GetComponent<SpriteRenderer>();
    }

    public override void Enter()
    {
        Entity.Direction = EntityDirection.Left;

        _spriteRenderer.flipX = false;
    }
    
    public override void Exit()
    {
        
    }
}
