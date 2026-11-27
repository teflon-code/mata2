using UnityEngine;
using UnityEngine.UI;

namespace NairobiMatatu
{
    public class MatatuHUD : MonoBehaviour
    {
        private Text speedText;
        private Text objectiveText;
        private Text instructionText;
        private MatatuController controller;
        private CheckpointRoute route;

        public void Setup(MatatuController matatu, CheckpointRoute matatuRoute)
        {
            controller = matatu;
            route = matatuRoute;

            speedText = Text("Speed", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -20f), 28, TextAnchor.UpperLeft);
            objectiveText = Text("Route", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -18f), 24, TextAnchor.UpperCenter);
            instructionText = Text("WASD / Arrows  Drive   Space  Brake   R  Reset   H  Horn",
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 12f), 18, TextAnchor.LowerCenter);

            objectiveText.color = new Color(1f, 0.85f, 0.20f);
            speedText.color = Color.white;
            instructionText.color = new Color(0.9f, 0.9f, 0.9f);
        }

        private void Update()
        {
            if (controller == null || route == null) return;
            speedText.text = Mathf.RoundToInt(controller.SpeedKph).ToString("0") + " km/h";

            if (route.Complete)
                objectiveText.text = "ROUTE COMPLETE  " + FormatTime(route.Elapsed);
            else
                objectiveText.text = "Checkpoint " + (route.CheckpointIndex + 1) + "/" + route.CheckpointCount +
                                    "  " + FormatTime(route.Elapsed);
        }

        private static string FormatTime(float seconds)
        {
            var minutes = Mathf.FloorToInt(seconds / 60f);
            var remaining = Mathf.FloorToInt(seconds % 60f);
            return minutes.ToString("00") + ":" + remaining.ToString("00");
        }

        private Text Text(string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 position, int size, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var rect = text.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = anchorMin;
            rect.sizeDelta = new Vector2(900f, 50f);
            rect.anchoredPosition = position;
            return text;
        }
    }
}
