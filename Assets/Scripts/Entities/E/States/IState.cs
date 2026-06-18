using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    string GetName { get; }
    bool IsFinished { get; }
    void OnEnter(EntityGeneral entity);
    void Tick(EntityGeneral entity, float deltaTime);
    void OnExit(EntityGeneral entity);
}
