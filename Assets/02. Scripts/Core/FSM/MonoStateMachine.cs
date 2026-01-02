using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoStateMachine<EntityType> : MonoBehaviour
{
    public delegate void StateChangedHandler(StateMachine<EntityType> stateMachine,
        State<EntityType> newState,
        State<EntityType> prevState,
        int layer);
    
    private readonly StateMachine<EntityType> stateMachine = new();

    public event StateChangedHandler onStateChanged;

    public EntityType Owner => stateMachine.Owner;

    private void Update()
    {
        if (Owner != null)
            stateMachine.Update();
        else
        {
            Debug.LogError($"Owner is null in {nameof(MonoStateMachine<EntityType>)}");
        }
    }

    public void Setup(EntityType owner)
    {
        stateMachine.Setup(owner);

        AddStates();
        MakeTransitions();
        Debug.Log("SetupLayers");
        stateMachine.SetupLayers();
        Debug.Log("SetupLayers done");

        stateMachine.onStateChanged += (_, newState, prevState, layer)
            => onStateChanged?.Invoke(stateMachine, newState, prevState, layer);
    }

    #region Overloadings
    public void AddState<T>(int layer = 0)
        where T : State<EntityType>
        => stateMachine.AddState<T>(layer);
    
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransitions<FromStateType, ToStateType>(transitionCommand, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransitions<FromStateType, ToStateType>(transitionCommand, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransitions<FromStateType, ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransitions<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => stateMachine.MakeTransitions<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeAnyTransition<ToStateType>(int transitionCommand, 
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransitions<ToStateType>(transitionCommand, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, 
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransitions<ToStateType>(transitionCommand, transitionCondition, layer, canTransitionToSelf);
    
    public void MakeAnyTransition<ToStateType>(Func<State<EntityType>, bool> transitionCondition, 
        int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransitions<ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransitions<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => stateMachine.MakeAnyTransitions<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);
        
    public bool ExecuteCommand(int transitionCommand, int layer)
        => stateMachine.ExecuteCommand(transitionCommand, layer);

    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => stateMachine.ExecuteCommand(transitionCommand, layer);
    
    public bool ExecuteCommand(int transitionCommand)
        => stateMachine.ExecuteCommand(transitionCommand);

    public bool SendMessage(int message, int layer, object extraData = null)
        => stateMachine.SendMessage(message, layer, extraData);

    public bool SendMessage(Enum message, int layer, object extraData = null)
        => stateMachine.SendMessage(message, layer, extraData);

    public bool SendMessage(int message, object extraData = null)
        => stateMachine.SendMessage(message, extraData);
    
    public bool SendMessage(Enum message, object extraData = null)
        => stateMachine.SendMessage(message, extraData);

    public bool IsInState<T>() where T : State<EntityType>
        => stateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<EntityType>
        => stateMachine.IsInState<T>(layer);

    public State<EntityType> GetCurrentState(int layer = 0)
        => stateMachine.GetCurrentState(layer);

    public Type GetCurrentStateType(int layer = 0)
        => stateMachine.GetCurrentStateType(layer);

    #endregion
    protected virtual void AddStates() { }

    protected virtual void MakeTransitions() { }
}
