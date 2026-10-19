using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WeatherModel", menuName = "Scriptable Objects/WeatherModel")]
public class WeatherModel : ScriptableObject
{
    [SerializeField] private float windSpeed;
    [SerializeField] private float windDirection;


    private void OnEnable()
    {
        windDirection = 0f;
        windSpeed = 0f;
    }

    public float WindSpeed
    {
        get => windSpeed;
        set => windSpeed = value;
    }

    public float WindDirection
    {
        get => windDirection;
        set => windDirection = value;
    }
}