using NaughtyAttributes;
using UnityEngine;

namespace BoatGame.Boat
{
    [CreateAssetMenu(fileName = "BoatSteeringModel", menuName = "Scriptable Objects/Boat/BoatSteeringModel")]
    public class BoatSteeringModel : ScriptableObject
    {
        [SerializeField] private float rudderAngle;
        [SerializeField] private float sailDeployment;
        [SerializeField] private float sailRotation;

        [SerializeField] [Expandable] private BoatSteeringResponsivenessModel steeringResponsivenessModel;

        public float RudderAngle => rudderAngle;
        public float SailDeployment => sailDeployment;
        public float SailRotation => sailRotation;
        public float RudderRotation101 => GetRudderRotation101();

        private void OnEnable()
        {
            rudderAngle = 0f;
            sailDeployment = 0f;
            sailRotation = 0f;
        }

        private void OnValidate()
        {
            rudderAngle = Mathf.Clamp(rudderAngle, -steeringResponsivenessModel.MaxRudderAngle,
                steeringResponsivenessModel.MaxRudderAngle);
            sailDeployment = Mathf.Clamp01(sailDeployment);
            sailRotation = Mathf.Clamp(sailRotation, -steeringResponsivenessModel.MaxSailAngle,
                steeringResponsivenessModel.MaxSailAngle);
        }

        private float GetRudderRotation101()
        {
            return rudderAngle / steeringResponsivenessModel.MaxRudderAngle;
        }

        public void UpdateRudderAngle(float deltaTime, float input)
        {
            rudderAngle += input * steeringResponsivenessModel.RudderDegreesPerSecond * deltaTime;
            rudderAngle = Mathf.Clamp(rudderAngle, -steeringResponsivenessModel.MaxRudderAngle,
                steeringResponsivenessModel.MaxRudderAngle);
        }

        public void UpdateSailRotation(float deltaTime, float input)
        {
            sailRotation += input * steeringResponsivenessModel.SailDegreesPerSecond * deltaTime;
            sailRotation = Mathf.Clamp(sailRotation, -steeringResponsivenessModel.MaxSailAngle,
                steeringResponsivenessModel.MaxSailAngle);
        }

        public void UpdateSailDeployment(float deltaTime, float input)
        {
            sailDeployment += input * steeringResponsivenessModel.SailDeploymentPercentPerSecond * deltaTime / 100f;
            sailDeployment = Mathf.Clamp01(sailDeployment);
        }
    }
}