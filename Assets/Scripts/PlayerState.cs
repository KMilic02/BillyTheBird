using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    class Transition
    {
        State state;
        State? previousState;
        Signal signal;
        State newState;

        public Transition(State state, Signal signal, State newState)
        {
            this.state = state;
            this.signal = signal;
            this.newState = newState;
        }

        public State getNewState()
        {
            return newState;
        }

        public bool test(State state, Signal signal)
        {
            return this.state == state && this.signal == signal;
        }
    }

    public PlayerState()
    {
        state = State.Grounded;
    }

    public PlayerState(State state)
    {
        this.state = state;
    }

    public static bool operator ==(PlayerState a, State b) => a.state == b;
    public static bool operator !=(PlayerState a, State b) => a.state != b;
    public static implicit operator State(PlayerState state) => state.state;

    State state;

    //New transitions go here
    //Current State, Signal, Next State
    readonly List<Transition> transitions = new List<Transition>
    {
        new Transition(State.Grounded, Signal.Jump, State.Jumping),
        new Transition(State.Jumping, Signal.Land, State.Grounded),
        new Transition(State.Jumping, Signal.Perch, State.Perched),
        new Transition(State.Perched, Signal.Jump, State.Jumping),
        new Transition(State.Jumping, Signal.Dash, State.Dashing),
        new Transition(State.Dashing, Signal.EndDash, State.Jumping),
    };

    public State getState()
    {
        return state;
    }

    public bool transitionTo(Signal signal)
    {
        var transition = getTransition();

        if (transition == null)
        {
            return false;
        }

        state = transition.getNewState();

        return true;

        Transition getTransition()
        {
            foreach (var transition in transitions)
            {
                if (transition.test(state, signal))
                    return transition;
            }

            return null;
        }
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (ReferenceEquals(obj, null))
        {
            return false;
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public enum State
{
    Grounded,
    Jumping,
    Perched,
    Dashing
}

public enum Signal
{
    Jump,
    Land,
    Perch,
    Dash,
    EndDash
}