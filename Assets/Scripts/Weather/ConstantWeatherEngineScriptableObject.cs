using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstantWeatherEngineScriptableObject",
    menuName = "Scriptable Objects/ConstantWeatherEngineScriptableObject")]
public class ConstantWeatherEngineScriptableObject : WeatherEngineScriptableObject
{
    [SerializeField] private float constantWindDirection = 0f;
    [SerializeField] private float constantWindSpeed = 0f;

    private CancellationTokenSource _weatherSimulationTaskCancellationTokenSource;
    
    public override void StartSimulation()
    {
        _weatherSimulationTaskCancellationTokenSource?.Cancel();
        _weatherSimulationTaskCancellationTokenSource = new CancellationTokenSource();

        SimulateWeatherChangesAsync(_weatherSimulationTaskCancellationTokenSource.Token).Forget();
    }

    public override void StopSimulation()
    {
        _weatherSimulationTaskCancellationTokenSource?.Cancel();
    }

    private void OnValidate()
    {
        if (constantWindSpeed < 0f)
        {
            Debug.LogWarning("Constant wind speed cannot be negative. Setting to 0.");
            constantWindSpeed = 0f;
        }

        constantWindDirection = NormalizeAngle(constantWindDirection);
        weatherModel.WindDirection = constantWindDirection;
    }
    
    private async UniTaskVoid SimulateWeatherChangesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            weatherModel.WindSpeed = constantWindSpeed;
            weatherModel.WindDirection = constantWindDirection;
            await UniTask.Yield();
        }
    }   
}