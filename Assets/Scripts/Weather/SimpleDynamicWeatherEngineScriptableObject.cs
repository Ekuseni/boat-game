using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SimpleDynamicWeatherEngineScriptableObject",
    menuName = "Scriptable Objects/SimpleDynamicWeatherEngineScriptableObject")]
public class SimpleDynamicWeatherEngineScriptableObject : WeatherEngineScriptableObject
{
    [SerializeField] private float windDirectionChangeRateMin = 1f; // degrees per second
    [SerializeField] private float windDirectionChangeRateMax = 10f; // degrees per second
    [SerializeField] private float windDirectionChangeIntervalMin = 5f; // seconds
    [SerializeField] private float windDirectionChangeIntervalMax = 15f; // seconds
    [SerializeField] private float windSpeedValueMin = 0f; // units
    [SerializeField] private float windSpeedValueMax = 10f; // units
    [SerializeField] private float windSpeedChangeRateMin = 0.1f; // units per second
    [SerializeField] private float windSpeedChangeRateMax = 1f; // units per second
    [SerializeField] private float windSpeedChangeIntervalMin = 5f; // seconds
    [SerializeField] private float windSpeedChangeIntervalMax = 15f; // seconds

    private CancellationTokenSource _weatherSimulationTaskCancellationTokenSource;

    [Button("Start Simulation")]
    public override void StartSimulation()
    {
        _weatherSimulationTaskCancellationTokenSource?.Cancel();
        _weatherSimulationTaskCancellationTokenSource = new CancellationTokenSource();

        SimulateWeatherChangesAsync(_weatherSimulationTaskCancellationTokenSource.Token).Forget();
    }

    [Button("Stop Simulation")]
    public override void StopSimulation()
    {
        _weatherSimulationTaskCancellationTokenSource?.Cancel();
    }

    private async UniTaskVoid SimulateWeatherChangesAsync(CancellationToken cancellationToken)
    {
        var windDirection = Random.Range(0f, 360f);
        var windSpeed = Random.Range(windSpeedValueMin, windSpeedValueMax);
        var directionChangeTimer = Random.Range(windDirectionChangeIntervalMin, windDirectionChangeIntervalMax);
        var directionChangeRate = Random.Range(windDirectionChangeRateMin, windDirectionChangeRateMax);
        var directionChangeSign = Random.value < 0.5f ? -1f : 1f;
        var speedChangeTimer = Random.Range(windSpeedChangeIntervalMin, windSpeedChangeIntervalMax);
        var speedChangeRate = Random.Range(windSpeedChangeRateMin, windSpeedChangeRateMax);
        var speedChangeSign = Random.value < 0.5f ? -1f : 1f;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (directionChangeTimer <= 0f)
            {
                directionChangeTimer = Random.Range(windDirectionChangeIntervalMin, windDirectionChangeIntervalMax);
                directionChangeSign = Random.value < 0.5f ? -1f : 1f;
            }
            else
            {
                directionChangeTimer -= Time.deltaTime;
            }

            if (speedChangeTimer <= 0f)
            {
                speedChangeTimer = Random.Range(windSpeedChangeIntervalMin, windSpeedChangeIntervalMax);
                speedChangeSign = Random.value < 0.5f ? -1f : 1f;
            }
            else
            {
                speedChangeTimer -= Time.deltaTime;
            }

            windDirection += directionChangeSign * directionChangeRate * Time.deltaTime;
            windDirection = NormalizeAngle(windDirection);
            windSpeed += speedChangeSign * speedChangeRate * Time.deltaTime;
            windSpeed = Mathf.Clamp(windSpeed, windSpeedValueMin, windSpeedValueMax);
            weatherModel.WindSpeed = windSpeed;
            weatherModel.WindDirection = windDirection;

            await UniTask.Yield();
        }
    }
}