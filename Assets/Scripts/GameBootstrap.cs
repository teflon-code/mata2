using UnityEngine;
using UnityEngine.UI;

namespace NairobiMatatu
{
    public class GameBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoCreate()
        {
            if (FindObjectOfType<GameBootstrap>() == null)
            {
                var go = new GameObject("GameBootstrap");
                go.AddComponent<GameBootstrap>();
            }
        }

        private void Awake()
        {
            BuildWorld();
        }

        private void BuildWorld()
        {
            var world = new GameObject("Nairobi CBD Map");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = "Nairobi Ground";
            ground.transform.SetParent(world.transform);
            ground.transform.localScale = new Vector3(500f, 1f, 500f);
            ground.transform.position = new Vector3(0f, -0.55f, 0f);
            ground.GetComponent<Renderer>().material = Mat("Ground", new Color(0.13f, 0.31f, 0.16f));

            foreach (var road in NairobiRoadData.Roads)
                BuildRoad(world, road);

            BuildCityBlocks(world);
            var matatu = BuildMatatu(world);

            var route = world.AddComponent<CheckpointRoute>();
            route.Setup(matatu.transform);

            var camera = new GameObject("Third Person Camera");
            var cam = camera.AddComponent<Camera>();
            cam.tag = "MainCamera";
            cam.fieldOfView = 65f;
            camera.AddComponent<AudioListener>();
            camera.AddComponent<ThirdPersonCamera>().Setup(matatu.transform);

            var sun = new GameObject("Sun");
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            sun.AddComponent<Light>().type = LightType.Directional;
            sun.GetComponent<Light>().intensity = 1.15f;
            sun.GetComponent<Light>().shadows = LightShadows.Soft;
            RenderSettings.ambientLight = new Color(0.48f, 0.52f, 0.56f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.72f, 0.82f, 0.80f);
            RenderSettings.fogDensity = 0.0018f;

            var hud = new GameObject("HUD");
            hud.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            hud.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            hud.AddComponent<GraphicRaycaster>();
            hud.AddComponent<MatatuHUD>().Setup(matatu.GetComponent<MatatuController>(), route);
        }

        private void BuildRoad(Transform world, RoadLine road)
        {
            var width = Mathf.Clamp(road.lanes * 3.2f, 6.5f, 13f);
            var group = new GameObject(road.name);
            group.transform.SetParent(world);

            for (var i = 0; i < road.points.Length - 1; i++)
            {
                var a = MapPoint(road.points[i]);
                var b = MapPoint(road.points[i + 1]);
                var segment = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segment.name = road.name + " segment";
                segment.transform.SetParent(group.transform);
                segment.transform.position = (a + b) * 0.5f + Vector3.up * 0.01f;
                segment.transform.localScale = new Vector3(width, 0.08f, Vector3.Distance(a, b) + 1f);
                segment.transform.LookAt(new Vector3(b.x, segment.transform.position.y, b.z));
                segment.GetComponent<Renderer>().material = Mat("Asphalt", new Color(0.09f, 0.10f, 0.11f));

                AddLaneMarkings(segment.transform, a, b, width);
            }
        }

        private void AddLaneMarkings(Transform segment, Vector3 a, Vector3 b, float width)
        {
            var direction = (b - a).normalized;
            var perpendicular = new Vector3(-direction.z, 0f, direction.x);
            var length = Vector3.Distance(a, b);
            for (var d = 3f; d < length; d += 6f)
            {
                var p = a + direction * d;
                var mark = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mark.name = "Lane marking";
                mark.transform.position = p + Vector3.up * 0.07f;
                mark.transform.localScale = new Vector3(0.12f, 0.02f, 2.2f);
                mark.transform.rotation = segment.rotation;
                mark.GetComponent<Renderer>().material = Mat("Road Marking", new Color(0.95f, 0.90f, 0.50f));
            }
        }

        private void BuildCityBlocks(Transform world)
        {
            var random = new System.Random(2607);
            for (var x = -210; x <= 210; x += 42)
            {
                for (var z = -210; z <= 210; z += 42)
                {
                    if (random.NextDouble() < 0.24) continue;
                    var building = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    building.name = "Nairobi Building";
                    building.transform.SetParent(world);
                    var height = 5f + (float)random.NextDouble() * 24f;
                    building.transform.position = new Vector3(x + (float)(random.NextDouble() * 8 - 4), height * 0.5f, z + (float)(random.NextDouble() * 8 - 4));
                    building.transform.localScale = new Vector3(18f + (float)random.NextDouble() * 8f, height, 18f + (float)random.NextDouble() * 8f);
                    var c = new Color(0.48f + (float)random.NextDouble() * 0.15f, 0.46f + (float)random.NextDouble() * 0.12f, 0.40f + (float)random.NextDouble() * 0.10f);
                    building.GetComponent<Renderer>().material = Mat("Concrete", c);
                }
            }
        }

        private GameObject BuildMatatu(Transform world)
        {
            var matatu = new GameObject("Player Matatu");
            matatu.transform.SetParent(world);
            matatu.transform.position = MapPoint(new Vector2(36.81635f, -1.28755f)) + Vector3.up * 1.4f;
            matatu.transform.rotation = Quaternion.Euler(0f, 170f, 0f);

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Matatu Body";
            body.transform.SetParent(matatu.transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localScale = new Vector3(2.6f, 1.35f, 5.2f);
            body.GetComponent<Renderer>().material = Mat("Matatu Green", new Color(0.08f, 0.55f, 0.28f));
            Object.Destroy(body.GetComponent<Collider>());
            matatu.AddComponent<BoxCollider>().size = new Vector3(2.7f, 1.5f, 5.3f);
            matatu.AddComponent<Rigidbody>().mass = 1450f;
            matatu.GetComponent<Rigidbody>().drag = 0.35f;
            matatu.GetComponent<Rigidbody>().angularDrag = 2.2f;
            matatu.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;

            var stripe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stripe.name = "Kenyan Flag Stripe";
            stripe.transform.SetParent(matatu.transform);
            stripe.transform.localPosition = new Vector3(0f, 0.18f, 0f);
            stripe.transform.localScale = new Vector3(2.68f, 0.12f, 5.25f);
            stripe.GetComponent<Renderer>().material = Mat("Stripe", new Color(0.98f, 0.56f, 0.05f));

            var windows = GameObject.CreatePrimitive(PrimitiveType.Cube);
            windows.name = "Windows";
            windows.transform.SetParent(matatu.transform);
            windows.transform.localPosition = new Vector3(0f, 0.55f, -0.15f);
            windows.transform.localScale = new Vector3(2.62f, 0.55f, 3.3f);
            windows.GetComponent<Renderer>().material = Mat("Windows", new Color(0.05f, 0.12f, 0.16f));

            matatu.AddComponent<MatatuController>();
            matatu.AddComponent<MatatuFeedback>();
            return matatu;
        }

        public static Vector3 MapPoint(Vector2 lonLat)
        {
            const float metersPerDegree = 111320f;
            const float lon0 = 36.8188f;
            const float lat0 = -1.2867f;
            float east = (lonLat.x - lon0) * metersPerDegree * Mathf.Cos(lat0 * Mathf.Deg2Rad);
            float north = (lonLat.y - lat0) * metersPerDegree;
            return new Vector3(east, 0f, north);
        }

        private static Material Mat(string name, Color color)
        {
            var mat = new Material(Shader.Find("Standard"));
            mat.name = name;
            mat.color = color;
            return mat;
        }
    }
}
