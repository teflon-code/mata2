using System.Collections.Generic;
using UnityEngine;

namespace NairobiMatatu
{
    public class CheckpointRoute : MonoBehaviour
    {
        private readonly Vector2[] route =
        {
            new Vector2(36.81595f,-1.28920f),
            new Vector2(36.82165f,-1.28910f),
            new Vector2(36.82165f,-1.28530f),
            new Vector2(36.81580f,-1.28435f),
            new Vector2(36.81555f,-1.29110f)
        };

        private readonly List<GameObject> checkpointObjects = new List<GameObject>();
        private Transform player;
        private int index;
        private float elapsed;
        private bool complete;

        public int CheckpointIndex => index;
        public int CheckpointCount => route.Length;
        public bool Complete => complete;
        public float Elapsed => elapsed;
        public Vector3 NextCheckpoint => checkpointObjects.Count > 0 ? checkpointObjects[Mathf.Min(index, checkpointObjects.Count - 1)].transform.position : Vector3.zero;

        public void Setup(Transform playerTransform)
        {
            player = playerTransform;
            for (var i = 0; i < route.Length; i++)
            {
                var point = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                point.name = "Checkpoint " + (i + 1);
                point.transform.position = GameBootstrap.MapPoint(route[i]) + Vector3.up * 1.2f;
                point.transform.localScale = new Vector3(7f, 1.2f, 7f);
                point.GetComponent<Collider>().isTrigger = true;
                var renderer = point.GetComponent<Renderer>();
                renderer.material = NewMat(i == 0 ? new Color(1f, 0.55f, 0.05f) : new Color(0.1f, 0.65f, 0.32f));
                point.AddComponent<CheckpointMarker>();
                checkpointObjects.Add(point);
            }
        }

        private void Update()
        {
            if (complete || player == null) return;
            elapsed += Time.deltaTime;

            if (index >= checkpointObjects.Count)
            {
                complete = true;
                return;
            }

            var target = checkpointObjects[index].transform;
            var distance = Vector3.Distance(player.position, target.position);
            if (distance < 9f)
            {
                target.gameObject.SetActive(false);
                index++;
                if (index < checkpointObjects.Count)
                {
                    checkpointObjects[index].GetComponent<Renderer>().material.color = new Color(1f, 0.55f, 0.05f);
                }
            }
        }

        private static Material NewMat(Color color)
        {
            var mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;
            mat.color = new Color(color.r, color.g, color.b, 0.65f);
            return mat;
        }
    }

    public class CheckpointMarker : MonoBehaviour
    {
        private void Update()
        {
            transform.Rotate(Vector3.up, 35f * Time.deltaTime);
        }
    }
}
