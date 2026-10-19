using NaughtyAttributes;
using UnityEngine;

public abstract class WeatherEngineScriptableObject : ScriptableObject
{
    [SerializeField] [Expandable] protected WeatherModel weatherModel;

    public abstract void StartSimulation();

    public abstract void StopSimulation();

    protected float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }
}