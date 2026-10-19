using UnityEngine;
using UnityEngine.InputSystem;

namespace BoatGame.Boat
{
    public class BoatView : MonoBehaviour
    {
        [SerializeField] private InputActionReference rudderInputAction;
        [SerializeField] private InputActionReference sailRotationInputAction;
        [SerializeField] private InputActionReference sailDeploymentInputAction;
        [SerializeField] private Transform rudderPivot;
        [SerializeField] private Transform boomPivot;
        [SerializeField] private Transform sailPivot;
        [SerializeField] private BoatWake boatWake;
        [SerializeField] private BoatSteeringModel boatSteeringModel;
        [SerializeField] private BoatSimulationModel boatSimulationModel;

        private void OnEnable()
        {
            rudderInputAction.action.Enable();
            sailRotationInputAction.action.Enable();
            sailDeploymentInputAction.action.Enable();
            boatSimulationModel.OnBoatPositionShifted += OnBoatPositionShifted;
        }
        
        private void Update()
        {
            var rudderInput = rudderInputAction.action.ReadValue<float>();
            var sailRotationInput = sailRotationInputAction.action.ReadValue<float>();
            var sailDeploymentInput = sailDeploymentInputAction.action.ReadValue<float>();

            boatSteeringModel.UpdateRudderAngle(Time.deltaTime, rudderInput);
            boatSteeringModel.UpdateSailRotation(Time.deltaTime, sailRotationInput);
            boatSteeringModel.UpdateSailDeployment(Time.deltaTime, sailDeploymentInput);
            
            SetRudder(boatSteeringModel.RudderAngle);
            SetBoom(boatSteeringModel.SailRotation);
            SetSail(boatSteeringModel.SailDeployment);
            UpdateTransformFromSimulation();
        }
        
        private void OnDisable()
        {
            rudderInputAction.action.Disable();
            sailRotationInputAction.action.Disable();
            sailDeploymentInputAction.action.Disable();
            boatSimulationModel.OnBoatPositionShifted -= OnBoatPositionShifted;
        }
        
        private void OnBoatPositionShifted(Vector2 shift)
        {
            boatWake.ShiftWake(shift);
        }

        private void SetRudder(float angle)
        {
            rudderPivot.localRotation = Quaternion.Euler(0, angle, 0);
        }

        private void SetBoom(float angle)
        {
            boomPivot.localRotation = Quaternion.Euler(0, angle, 0);
        }

        private void SetSail(float deployment)
        {
            sailPivot.localScale = new Vector3(1, deployment, 1);
        }

        private void UpdateTransformFromSimulation()
        {
            var updatedTransform =
                boatSimulationModel.UpdateBoatSimulation(Time.deltaTime, transform);

            transform.position = updatedTransform.position;
            transform.rotation = updatedTransform.rotation;
        }
    }
}