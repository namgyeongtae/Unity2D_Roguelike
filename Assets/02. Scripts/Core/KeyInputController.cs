using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ActionId
{
    MoveUp, MoveDown, MoveLeft, MoveRight,
    Attack, Dash, Interact, Dodge,
    Item0, Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8, Item9, Item10, Item11
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
        Bind(ActionId.Dodge, KeyCode.Space);

        Bind(ActionId.Item0, KeyCode.Alpha1);
        Bind(ActionId.Item1, KeyCode.Alpha2);
        Bind(ActionId.Item2, KeyCode.Alpha3);
        Bind(ActionId.Item3, KeyCode.Alpha4);
        Bind(ActionId.Item4, KeyCode.Alpha5);
        Bind(ActionId.Item5, KeyCode.Alpha6);
        Bind(ActionId.Item6, KeyCode.Alpha7);
        Bind(ActionId.Item7, KeyCode.Alpha8);
        Bind(ActionId.Item8, KeyCode.Alpha9);
        Bind(ActionId.Item9, KeyCode.Alpha0);
        Bind(ActionId.Item10, KeyCode.Minus);
        Bind(ActionId.Item11, KeyCode.KeypadEquals);
    }

    private void Start()
    {
        SetCommand(ActionId.MoveUp, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.up));
        SetCommand(ActionId.MoveDown, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.down));
        SetCommand(ActionId.MoveLeft, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.left));
        SetCommand(ActionId.MoveRight, InputPhase.Hold, () => onDirectionInput?.Invoke(Vector2.right));

        SetCommand(ActionId.MoveUp, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));
        SetCommand(ActionId.MoveDown, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));
        SetCommand(ActionId.MoveLeft, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));
        SetCommand(ActionId.MoveRight, InputPhase.Up, () => onDirectionInput?.Invoke(Vector2.zero));

        var inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        if (!inventory)
        {
            Debug.LogError("KeyInputController::Start - Inventory not found");
            return;
        }

        SetCommand(ActionId.Item0, InputPhase.Down, () => inventory.UseItem(0));
        SetCommand(ActionId.Item1, InputPhase.Down, () => inventory.UseItem(1));
        SetCommand(ActionId.Item2, InputPhase.Down, () => inventory.UseItem(2));
        SetCommand(ActionId.Item3, InputPhase.Down, () => inventory.UseItem(3));
        SetCommand(ActionId.Item4, InputPhase.Down, () => inventory.UseItem(4));
        SetCommand(ActionId.Item5, InputPhase.Down, () => inventory.UseItem(5));
        SetCommand(ActionId.Item6, InputPhase.Down, () => inventory.UseItem(6));
        SetCommand(ActionId.Item7, InputPhase.Down, () => inventory.UseItem(7));
        SetCommand(ActionId.Item8, InputPhase.Down, () => inventory.UseItem(8));
        SetCommand(ActionId.Item9, InputPhase.Down, () => inventory.UseItem(9));
        SetCommand(ActionId.Item10, InputPhase.Down, () => inventory.UseItem(10));
        SetCommand(ActionId.Item11, InputPhase.Down, () => inventory.UseItem(11));
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
                
            break;
        }
    }
}
