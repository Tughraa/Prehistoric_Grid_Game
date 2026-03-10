using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusEffect
{
    void OnApply(EntityGeneral entity);
    void OnRemove(EntityGeneral entity);
    void Tick(EntityGeneral entity, float deltaTime);
    public IStatusEffect Clone();
    public void RefreshFrom(IStatusEffect other);
    bool IsFinished { get; }
    bool hasParticles { get; }
    Color GetColor { get; }
    string GetMessage { get; }
}
