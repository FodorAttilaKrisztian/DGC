using UnityEngine;

public abstract class PowerupEffect : ScriptableObject
{
    public abstract bool Apply(GameObject target);
}