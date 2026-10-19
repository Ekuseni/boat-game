using UnityEngine;

namespace BoatGame.Boat
{
    [CreateAssetMenu(fileName = "BoatSteeringResponsivenessModel",
        menuName = "Scriptable Objects/Boat/BoatSteeringResponsivenessModel")]
    public class BoatSteeringResponsivenessModel : ScriptableObject
    {
        [SerializeField] private float rudderDegreesPerSecond = 5f;
        [SerializeField] private float maxRudderAngle = 30f;
        [SerializeField] private float sailDegreesPerSecond = 5f;
        [SerializeField] private float maxSailAngle = 30f;
        [SerializeField] private float sailDeploymentPercentPerSecond = 5f;

        public float RudderDegreesPerSecond => rudderDegreesPerSecond;
        public float MaxRudderAngle => maxRudderAngle;
        public float SailDegreesPerSecond => sailDegreesPerSecond;
        public float MaxSailAngle => maxSailAngle;
        public float SailDeploymentPercentPerSecond => sailDeploymentPercentPerSecond;
    }
}