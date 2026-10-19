using BoatGame.Boat;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private BoatSimulationModel boatSimulationModel;
    [SerializeField] private Transform cameraTransform;
    private Vector3 _cameraOffset;


    private void Start()
    {
        _cameraOffset = cameraTransform.position -
                        new Vector3(boatSimulationModel.BoatPosition.x, 0f, boatSimulationModel.BoatPosition.y);
    }

    private void Update()
    {
        cameraTransform.position =
            new Vector3(boatSimulationModel.BoatPosition.x, 0f, boatSimulationModel.BoatPosition.y) + _cameraOffset;
    }
}