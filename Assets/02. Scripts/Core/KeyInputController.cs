using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActionId
{
    MoveUp, MoveDown, MoveLeft, MoveRight,
    Attack, Dash, Interact
}

public enum InputPhase { Down, Up, Hold }

public class KeyInputController : MonoBehaviour
{
    private static KeyInputController _instance;

    public static KeyInputController Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<KeyInputController>();
            return _instance;
        }
    }

    private readonly Dictionary<(ActionId, InputPhase), Action> _commands = new();
    private readonly Dictionary<(ActionId, InputPhase), Action<Vector2>> _directionCommands = new();

    private readonly Dictionary<ActionId, KeyCode> _bindings = new();

    private readonly HashSet<KeyCode> _usedKeys = new();

    public event Action<Vector2> onDirectionInput;

    public void Bind(ActionId actionId, KeyCode key)
    {
        if (_bindings.TryGetValue(actionId, out var existingKey))
        {
            if (existingKey == key)
                return;
            
            _usedKeys.Remove(existingKey);
        }

        _bindings[actionId] = key;
        _usedKeys.Add(key);
    }

    public KeyCode GetBinding(ActionId actionId) => _bindings[actionId];

    public void SetCommand(ActionId actionId, InputPhase phase, Action command)
    {
        _commands[(actionId, phase)] = command;
    }
    private void Awake()
    {
        Bind(ActionId.MoveUp, KeyCode.W);
        Bind(ActionId.MoveDown, KeyCode.S);
        Bind(ActionId.MoveLeft, KeyCode.A);
        Bind(ActionId.MoveRight, KeyCode.D);

        Bind(ActionId.Attack, KeyCode.Mouse0);
        Bind(ActionId.Dash, KeyCode.LeftShift);
        Bind(ActionId.Interact, KeyCode.F);
    }

    private void Start()
    {
        /* SetCommand(ActionId.Attack, InputPhase.Down, () => Debug.Log("Attack"));
        SetCommand(ActionId.Dash, InputPhase.Down, () => Debug.Log("Dash"));
        SetCommand(ActionId.Interact, InputPhase.Down, () => Debug.Log("Interact")); */

        SetCommand(ActionId.MoveUp, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.up));
        SetCommand(ActionId.MoveDown, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.down));
        SetCommand(ActionId.MoveLeft, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.left));
        SetCommand(ActionId.MoveRight, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.right));

        SetCommand(ActionId.MoveUp, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));
        SetCommand(ActionId.MoveDown, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));
        SetCommand(ActionId.MoveLeft, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));
        SetCommand(ActionId.MoveRight, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));
    }

    private void Update()
    {
        foreach (var key in _usedKeys)
        {
            if (Input.GetKeyDown(key)) InvokeByKey(key, InputPhase.Down);
            if (Input.GetKeyUp(key)) InvokeByKey(key, InputPhase.Up);
            if (Input.GetKey(key)) InvokeByKey(key, InputPhase.Hold);
        }
    }

    private void InvokeByKey(KeyCode key, InputPhase phase)
    {
        foreach (var pair in _bindings)
        {
            if (pair.Value != key) continue;

            if (_commands.TryGetValue((pair.Key, phase), out var command))
                command?.Invoke();

            /* if (_directionCommands.TryGetValue((pair.Key, phase), out var directionCommand))
                directionCommand?.Invoke(Vector2.zero); */
            
            break;
        }
    }
}
