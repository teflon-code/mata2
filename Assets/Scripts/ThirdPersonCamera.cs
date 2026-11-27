using UnityEngine;

namespace NairobiMatatu
{
    public class ThirdPersonCamera : MonoBehaviour
    {
        private Transform target;
        private float distance = 10f;
        private float height = 5.2f;
        private float lookHeight = 1.5f;
        private float smooth = 7f;

        public void Setup(Transform matatu)
        {
            target = matatu;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            var desired = target.position - target.forward * distance + Vector3.up * height;
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-smooth * Time.deltaTime));
            var lookAt = target.position + Vector3.up * lookHeight;
            transform.LookAt(lookAt);
        }
    }
}
