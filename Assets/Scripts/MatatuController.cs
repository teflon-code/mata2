using UnityEngine;

namespace NairobiMatatu
{
    [RequireComponent(typeof(Rigidbody))]
    public class MatatuController : MonoBehaviour
    {
        [SerializeField] private float acceleration = 18f;
        [SerializeField] private float reverseAcceleration = 9f;
        [SerializeField] private float steering = 1.8f;
        [SerializeField] private float brakeForce = 30f;
        [SerializeField] private float maxSpeed = 22f;
        [SerializeField] private float offRoadLimit = 15f;

        private Rigidbody body;
        private float throttle;
        private float steer;
        private float speed;

        public float Speed => speed;
        public float SpeedKph => Mathf.Abs(speed) * 3.6f;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            body.centerOfMass = new Vector3(0f, -0.65f, 0f);
        }

        private void Update()
        {
            throttle = Input.GetAxisRaw("Vertical");
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) &&
                !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) &&
                Mathf.Abs(throttle) < 0.01f)
                throttle = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) throttle = 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) throttle = -1f;

            steer = Input.GetAxisRaw("Horizontal");
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) steer = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) steer = 1f;
            if (Input.GetKey(KeyCode.R)) ResetVehicle();
        }

        private void FixedUpdate()
        {
            var localVelocity = transform.InverseTransformDirection(body.velocity);
            speed = localVelocity.z;

            if (Input.GetKey(KeyCode.Space))
            {
                body.velocity *= 1f - Mathf.Min(brakeForce * Time.fixedDeltaTime, 0.25f);
            }

            if (Mathf.Abs(speed) < maxSpeed)
            {
                var force = throttle >= 0f ? acceleration : reverseAcceleration;
                body.AddRelativeForce(Vector3.forward * throttle * force, ForceMode.Acceleration);
            }

            if (Mathf.Abs(speed) > 0.25f)
            {
                var speedFactor = Mathf.Clamp01(Mathf.Abs(speed) / 8f);
                var direction = Mathf.Sign(speed);
                body.AddTorque(Vector3.up * steer * steering * speedFactor * direction, ForceMode.Acceleration);
            }

            body.velocity = Vector3.ClampMagnitude(body.velocity, maxSpeed);
        }

        private void ResetVehicle()
        {
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            transform.position = GameBootstrap.MapPoint(new Vector2(36.81635f, -1.28755f)) + Vector3.up * 2f;
            transform.rotation = Quaternion.Euler(0f, 170f, 0f);
        }
    }
}
