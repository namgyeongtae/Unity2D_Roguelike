using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Entity _entity;

    void Start()
    {
        _entity = GetComponent<Entity>();
    }

    void OnEnable()
        => KeyInputController.Instance.onKeyInputDown += Move;

    void OnDisable()
        => KeyInputController.Instance.onKeyInputDown -= Move;
    void Move(Vector2 dir)
    {
        _entity.Movement?.Move(dir);
    }
}
