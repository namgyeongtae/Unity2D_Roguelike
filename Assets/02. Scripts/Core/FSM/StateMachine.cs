using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework.Internal;
using System.Security.Cryptography;
using UnityEditor.VersionControl;
using System.Data.Common;

public class StateMachine<EntityType>
{
    #region Event
    public delegate void StateChangedHandler(StateMachine<EntityType> stateMachine, 
        State<EntityType> newState,
        State<EntityType> prevState,
        int layer);
    #endregion

    private class StateData
    {
        public int Layer { get; private set; }
        public int Priority { get; private set; }
        public State<EntityType> State { get; private set; }
        public List<StateTransition<EntityType>> Transitions { get; private set; } = new();

        public StateData(int layer, int priority, State<EntityType> state)
            => (Layer, Priority, State) = (layer, priority, state);
    }

    private readonly Dictionary<int, Dictionary<Type, StateData>> stateDatasByLayer = new();

    private readonly Dictionary<int, List<StateTransition<EntityType>>> anyTransitionsByLayer = new();

    private readonly Dictionary<int, StateData> currentStateDataByLayer = new();

    private SortedSet<int> layers = new();

    public EntityType Owner { get; private set; }

    public event StateChangedHandler onStateChanged;

    public void Setup(EntityType owner)
    {
        Owner = owner;

        AddStates();
        MakeTransitions();
        SetupLayers();
    }

    public void SetupLayers()
    {
        foreach ((int layer, var stateDatas) in stateDatasByLayer)
        {
            currentStateDataByLayer[layer] = null;

            var firstStateData = stateDatas.Values.First(x => x.Priority == 0);

            ChangeState(firstStateData);
        }
    }

    private void ChangeState(StateData newStateData)
    {
        var prevStateData = currentStateDataByLayer[newStateData.Layer];

        prevStateData?.State.Exit();
        currentStateDataByLayer[newStateData.Layer] = newStateData;
        newStateData.State.Enter();

        onStateChanged?.Invoke(this, newStateData.State, prevStateData.State, newStateData.Layer);
    }

    private void ChangeState(State<EntityType> newState, int layer)
    {
        var newStateData = stateDatasByLayer[layer][newState.GetType()];

        ChangeState(newStateData);
    }

    private bool TryTransition(IReadOnlyList<StateTransition<EntityType>> transitions, int layer)
    {
        foreach (var transition in transitions)
        {
            // Command가 존재하거나 전환 가능하지 않은은 경우
            if (transition.TransitionCommand != StateTransition<EntityType>.kNullCommand || !transition.IsTransferable)
                continue;

            if (!transition.CanTransitionToSelf && currentStateDataByLayer[layer].State == transition.ToState)
                continue;

            ChangeState(transition.ToState, layer);
            return true;
        }

        return false;
    }

    public void Update()
    {
        foreach (var layer in layers)
        {
            var currentStateData = currentStateDataByLayer[layer];

            bool hasAnyTransition = anyTransitionsByLayer.TryGetValue(layer, out var anyTransitions);

            if ((hasAnyTransition && TryTransition(anyTransitions, layer)) ||
                TryTransition(currentStateData.Transitions, layer))
                continue;

            currentStateData.State.Update();
        }
    }

    // layer에 T Type의 State를 추가해야함
    public void AddState<T>(int layer = 0) where T : State<EntityType>
    {
        layers.Add(layer);

        var newState = Activator.CreateInstance<T>();
        newState.Setup(this, Owner, layer);

        // layer가 없는 경우는 nlayer를 만들어 주어야 함
        if (!stateDatasByLayer.ContainsKey(layer))
        {
            stateDatasByLayer[layer] = new();
            anyTransitionsByLayer[layer] = new();
        }
        
        var stateDatas = stateDatasByLayer[layer];

        stateDatas[typeof(T)] = new StateData(layer, stateDatas.Count, newState);
    }

    public void MakeTransitions<FromStateType, ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition,
        int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
    {
        var stateDatas = stateDatasByLayer[layer];

        var fromStateData = stateDatas[typeof(FromStateType)];
        var toStateData = stateDatas[typeof(ToStateType)];

        var newTransition = new StateTransition<EntityType>(fromStateData.State, toStateData.State, transitionCommand, transitionCondition, true);
    
        fromStateData.Transitions.Add(newTransition);
    }

    public void MakeTransitions<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransitions<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer);

    public void MakeTransitions<FromStateType, ToStateType>(Func<State<EntityType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransitions<FromStateType, ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer);

    public void MakeTransitions<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransitions<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeTransitions<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<EntityType>
        where ToStateType : State<EntityType>
        => MakeTransitions<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeAnyTransitions<ToStateType>(int transitionCommand,
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
    {
        var stateDatas = stateDatasByLayer[layer];

        var toStateData = stateDatas[typeof(ToStateType)];

        var newTransition = new StateTransition<EntityType>(null, toStateData.State, transitionCommand, transitionCondition, canTransitionToSelf);
    
        anyTransitionsByLayer[layer].Add(newTransition);
    }

    public void MakeAnyTransitions<ToStateType>(Enum transitionCommand, 
        Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransitions<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransitions<ToStateType>(Func<State<EntityType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransitions<ToStateType>(StateTransition<EntityType>.kNullCommand, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransitions<ToStateType>(int transitionCommand, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransitions<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    public void MakeAnyTransitions<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<EntityType>
        => MakeAnyTransitions<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    public bool ExecuteCommand(int transitionCommand, int layer)
    {
        var transition = anyTransitionsByLayer[layer].Find(x => x.TransitionCommand == transitionCommand && x.IsTransferable);

        transition ??= currentStateDataByLayer[layer].Transitions.Find(x => x.TransitionCommand == transitionCommand && x.IsTransferable);

        if (transition == null)
            return false;
        
        ChangeState(transition.ToState, layer);
        return true;
    }

    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => ExecuteCommand(Convert.ToInt32(transitionCommand), layer);

    public bool ExecuteCommand(int transitionCommand)
    {
        bool isSuccess = false;

        foreach (var layer in layers)
        {
            if (ExecuteCommand(transitionCommand, layer))
                isSuccess = true;
        }

        return isSuccess;
    }

    public bool SendMessage(int message, int layer, object extraData = null)
        => currentStateDataByLayer[layer].State.OnReceiveMessage(message, extraData);

    public bool SendMessage(Enum message, int layer, object extraData = null)
        => SendMessage(Convert.ToInt32(message), layer, extraData);

    public bool SendMessage(int message, object extraData = null)
    {
        bool isSuccess = false;

        foreach (var layer in layers)
        {
            if (SendMessage(message, layer, extraData))
                isSuccess = true;
        }

        return isSuccess;
    }

    public bool SendMessage(Enum message, object extraData = null)
        => SendMessage(Convert.ToInt32(message), extraData);

    public bool IsInState<T>() where T : State<EntityType>
    {
        foreach ((_, StateData data) in currentStateDataByLayer)
        {
            if (data.State.GetType() == typeof(T))
                return true;
        }

        return false;
    }

    public bool IsInState<T>(int layer) where T : State<EntityType>
        => currentStateDataByLayer[layer].State.GetType() == typeof(T);

    public State<EntityType> GetCurrentState(int layer = 0)
        => currentStateDataByLayer[layer].State;

    public Type GetCurrentStateType(int layer = 0)
        => currentStateDataByLayer[layer].State.GetType();

    protected virtual void AddStates() { }
    protected virtual void MakeTransitions() { }
}
