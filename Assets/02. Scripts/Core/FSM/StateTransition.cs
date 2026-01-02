using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateTransition<EntityType>
{
    public const int kNullCommand = int.MinValue;

    public int TransitionCommand { get; private set; }

    public Func<State<EntityType>, bool> transitionCondition { get; private set; }

    public bool CanTransitionToSelf { get; private set; }

    public State<EntityType> FromState { get; private set; }
    public State<EntityType> ToState { get; private set; }

    public bool IsTransferable => transitionCondition == null || transitionCondition.Invoke(FromState);

    public StateTransition(State<EntityType> fromState, 
        State<EntityType> toState, 
        int transitionCommand, 
        Func<State<EntityType>, bool> transitionCondition, 
        bool canTransitionToSelf)
    {
        FromState = fromState;
        ToState = toState;
        TransitionCommand = transitionCommand;
        this.transitionCondition = transitionCondition;
        CanTransitionToSelf = canTransitionToSelf;
    }
}
