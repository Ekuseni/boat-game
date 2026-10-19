using BoatGame.Boat;
using NaughtyAttributes;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "MainUIDataBindingScriptableObject",
    menuName = "Scriptable Objects/MainUIDataBindingScriptableObject")]
public class MainUIDataBindingScriptableObject : ScriptableObject
{
    [CreateProperty] public string WindSpeed => WindSpeedToString();
    [CreateProperty] public StyleRotate WindDirection => WindDirectionToStyleRotate();
    [CreateProperty] public StyleScale SailDeployment => SailDeploymentToVector3();
    [CreateProperty] public StyleRotate SailRotation => SailRotationToStyleRotate();
    [CreateProperty] public StyleScale RudderRotation => RudderRotationToStyleScale();
    [CreateProperty] public StyleRotate BoatRotation => BoatRotationToStyleRotate();

    [SerializeField] [Expandable] private WeatherModel weatherModel;
    [SerializeField] [Expandable] private BoatSteeringModel steeringModel;
    [SerializeField] [Expandable] private BoatSimulationModel boatSimulationModel;

    private string WindSpeedToString()
    {
        return $"{weatherModel?.WindSpeed:0.0} m/s";
    }

    private StyleRotate WindDirectionToStyleRotate()
    {
        return new Rotate(new Angle(weatherModel?.WindDirection ?? 0f, AngleUnit.Degree));
    }

    private StyleScale SailDeploymentToVector3()
    {
        return new StyleScale(new Vector3(1f, steeringModel?.SailDeployment ?? 0f, 1f));
    }

    private StyleRotate SailRotationToStyleRotate()
    {
        return new StyleRotate(new Angle(steeringModel?.SailRotation ?? 0f, AngleUnit.Degree));
    }

    private StyleScale RudderRotationToStyleScale()
    {
        return new StyleScale(new Vector3(-steeringModel?.RudderRotation101 ?? 0f, 1f, 1f));
    }

    private StyleRotate BoatRotationToStyleRotate()
    {
        return new StyleRotate(new Angle(boatSimulationModel?.BoatRotation ?? 0f, AngleUnit.Degree));
    }
}