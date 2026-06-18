using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    public EntityGeneral entity;
    IState startState;
    public IState state;
    void Start()
    {
        entity = this.GetComponent<EntityGeneral>();
        //InitState(startState);
    }
    void Update()
    {
        state.Tick(entity,Time.deltaTime);
    }
    public void InitState(IState inStartState)
    {
        startState = inStartState;
        startState.OnEnter(entity);
        state = startState;
    }
    public void StateTransition(IState nextState)
    {
        state.OnExit(entity);//Exit the last state
        state = nextState;   //Change the current state
        state.OnEnter(entity);
    }
}
