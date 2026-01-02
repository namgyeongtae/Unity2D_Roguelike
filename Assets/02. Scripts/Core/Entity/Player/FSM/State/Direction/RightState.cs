using UnityEngine;

public class RightState : State<Entity>
{
    private SpriteRenderer _spriteRenderer;

    protected override void Setup()
    {
        _spriteRenderer = Entity.GetComponent<SpriteRenderer>();
    }

    public override void Enter()
    {
        Entity.Direction = EntityDirection.Right;

        _spriteRenderer.flipX = true;
    }
    
    public override void Exit()
    {
        
    }
}
