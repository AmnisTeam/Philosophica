using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public abstract class Condition
{
    public abstract bool CheckCondition();
}

public class Transition
{
    public Condition condition;
    public int order;
    public int from;
    public int to;

    public Transition(Condition condition, int from, int to)
    {
        this.condition = condition;
        this.from = from;
        this.to = to;
    }

    public Transition(Condition condition, State from, State to, StateMachine stateMachine)
    {
        this.condition = condition;
        this.from = FindStateId(stateMachine, from);
        this.to = FindStateId(stateMachine, to);
    }

    private int FindStateId(StateMachine stateMachine, State state)
    {
        for (int s = 0; s < stateMachine.states.Count; s++)
            if (state == stateMachine.states[s])
                return s;
        return -1;
    }
}

public delegate void StateEvent();
public class State
{
    public StateEvent startEvents;
    public StateEvent updateEvents;
}

public class StateMachine
{
    public List<State> states = new List<State>();
    public List<Transition> transitions = new List<Transition>();
    public int activeState = 0;

    public StateMachine()
    {
    }

    public void Start(int activeState)
    {
        this.activeState = activeState;
        states[activeState].startEvents();
    }

    public void UpdateEvents()
    {
        states[activeState].updateEvents();
    }

    public void UpdateConditions()
    {
        CheckConditions();
    }

    public void CheckConditions()
    {
        for (int i = 0; i < transitions.Count; i++)
        {
            if (transitions[i].from == activeState)
                if (transitions[i].condition.CheckCondition())
                {
                    activeState = transitions[i].to;
                    states[activeState].startEvents();
                }             
        }
    }
}
