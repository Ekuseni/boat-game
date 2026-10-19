using NaughtyAttributes;
using UnityEngine;

namespace BoatGame.Boat
{
    [CreateAssetMenu(fileName = "BoatSimulationModel", menuName = "Scriptable Objects/Boat/BoatSimulationModel")]
    public class BoatSimulationModel : ScriptableObject
    {
        [SerializeField] [Expandable] private WeatherModel weatherModel;
        [SerializeField] [Expandable] private BoatSteeringModel steeringModel;

        [SerializeField] private float boatMass = 100f;
        [SerializeField] private float boatRotation;
        [SerializeField] private Vector2 boatWaterDrag;
        [SerializeField] private Vector2 boatWindDrag;
        [SerializeField] private float boatSailForce;
        [SerializeField] private Vector2 boatSailForceAxialScaleFactors;
        [SerializeField, Range(1f, 89f)] private float boatSailStallAngle = 25f;
        [SerializeField] private float boatTurnRate = 20f;
        [SerializeField] private Vector2 boatPosition;
        [SerializeField] private Vector2 boatVelocity;

        public float BoatRotation => boatRotation;
        public Vector2 BoatPosition => boatPosition;
        
        public event System.Action<Vector2> OnBoatPositionShifted;

        private void OnEnable()
        {
            boatRotation = 0f;
            boatPosition = Vector2.zero;
            boatVelocity = Vector2.zero;
        }

        public void ShiftBoatPosition(Vector2 shift)
        {
            boatPosition += shift;
            OnBoatPositionShifted?.Invoke(shift);
        }

        public Transform UpdateBoatSimulation(float deltaTime, Transform boatTransform)
        {
            boatRotation = Mathf.Repeat(boatTransform.rotation.eulerAngles.y, 360f);
            var boatForward = new Vector2(Mathf.Sin(boatRotation * Mathf.Deg2Rad),
                Mathf.Cos(boatRotation * Mathf.Deg2Rad));
            var boatRight = new Vector2(boatForward.y, -boatForward.x);
            var windDirection = new Vector2(Mathf.Sin(weatherModel.WindDirection * Mathf.Deg2Rad),
                Mathf.Cos(weatherModel.WindDirection * Mathf.Deg2Rad));
            var windVelocity = windDirection * weatherModel.WindSpeed;
            var apparentWind = windVelocity - boatVelocity;
            var sailHeading = boatRotation + steeringModel.SailRotation;
            var sailForward = new Vector2(Mathf.Sin(sailHeading * Mathf.Deg2Rad),
                Mathf.Cos(sailHeading * Mathf.Deg2Rad));
            var sailTack = Vector2.SignedAngle(boatForward, sailForward);
            var sailNormal = new Vector2(-sailForward.y, sailForward.x) * Mathf.Sign(sailTack);
            var sailForce = CalculateSailForce(apparentWind, sailForward, sailNormal);
            
            sailForce = boatForward * (Vector2.Dot(sailForce, boatForward) * boatSailForceAxialScaleFactors.y)
                        + boatRight * (Vector2.Dot(sailForce, boatRight) * boatSailForceAxialScaleFactors.x);

            var forwardSpeed = Vector2.Dot(boatVelocity, boatForward);
            var sidewaysSpeed = Vector2.Dot(boatVelocity, boatRight);
            var waterDragForce = -boatForward * (forwardSpeed * Mathf.Abs(forwardSpeed) * boatWaterDrag.y)
                                 - boatRight * (sidewaysSpeed * Mathf.Abs(sidewaysSpeed) * boatWaterDrag.x);

            var apparentWindForward = Vector2.Dot(apparentWind, boatForward);
            var apparentWindSideways = Vector2.Dot(apparentWind, boatRight);
            var windDragForce = boatForward * (apparentWindForward * Mathf.Abs(apparentWindForward) * boatWindDrag.y)
                                + boatRight * (apparentWindSideways * Mathf.Abs(apparentWindSideways) * boatWindDrag.x);

            var speedFactor = boatVelocity.magnitude / (boatVelocity.magnitude + 1f);
            boatRotation = Mathf.Repeat(
                boatRotation - steeringModel.RudderRotation101 * boatTurnRate * speedFactor * deltaTime, 360f);

            var totalForce = sailForce + waterDragForce + windDragForce;
           
            DrawDebugVector(boatTransform.position, boatForward, Color.blue, 1f);
            DrawDebugVector(boatTransform.position, windDirection, Color.red, 1f);
            DrawDebugVector(boatTransform.position, windVelocity, Color.red, 0.25f);
            DrawDebugVector(boatTransform.position, apparentWind, Color.magenta, 0.25f);

            DrawDebugVector(boatTransform.position, sailForward, Color.white, 1f);
            DrawDebugVector(boatTransform.position, sailNormal, Color.gray, 0.5f);
            DrawDebugVector(boatTransform.position, sailForce, Color.green, 2f);
            DrawDebugVector(boatTransform.position, waterDragForce, Color.yellow, 0.1f);
            DrawDebugVector(boatTransform.position, windDragForce, new Color(1f, 0.5f, 0f), 0.1f);
            DrawDebugVector(boatTransform.position, boatVelocity, Color.cyan, 0.25f);
            
            boatVelocity += totalForce / Mathf.Max(boatMass, 0.001f) * deltaTime;
            boatPosition += boatVelocity * deltaTime;
            
            var position = boatTransform.position;
            boatTransform.position = new Vector3(boatPosition.x, position.y, boatPosition.y);
            boatTransform.rotation = Quaternion.Euler(0f, boatRotation, 0f);

            return boatTransform;
        }

        private Vector2 CalculateSailForce(Vector2 apparentWind, Vector2 sailForward, Vector2 sailNormal)
        {
            if (steeringModel.SailDeployment <= 0f || apparentWind.sqrMagnitude < 0.0001f )
            {
                return Vector2.zero;
            }
            
            var apparentWindAngle = Vector2.Angle(-apparentWind, sailForward);
            
            if (apparentWindAngle < boatSailStallAngle || apparentWindAngle > 180f - boatSailStallAngle)
            {
                return Vector2.zero;
            }

            var windDirection = apparentWind.normalized;
            var sailDot = Vector2.Dot(sailNormal, windDirection);

            if (sailDot <= 0f)
            {
                var sailForce = sailNormal * (apparentWind.magnitude * boatSailForce * steeringModel.SailDeployment);
                var scaleFactor = (90f - Vector2.Angle(-sailNormal, windDirection)) / 90f;
                Debug.Log(scaleFactor);
                sailForce *= scaleFactor;
                return -sailForce;
            }

            return Vector2.zero;
        }

        private static void DrawDebugVector(Vector3 origin, Vector2 vector, Color color, float scale)
        {
            Debug.DrawLine(origin, origin + new Vector3(vector.x, 0f, vector.y) * scale, color);
        }
    }
}