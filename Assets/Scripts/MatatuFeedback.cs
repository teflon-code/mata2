using UnityEngine;

namespace NairobiMatatu
{
    [RequireComponent(typeof(MatatuController))]
    public class MatatuFeedback : MonoBehaviour
    {
        private MatatuController controller;
        private AudioSource engine;
        private AudioSource horn;

        private void Awake()
        {
            controller = GetComponent<MatatuController>();
            engine = gameObject.AddComponent<AudioSource>();
            engine.loop = true;
            engine.volume = 0.12f;
            engine.clip = CreateEngineTone();
            engine.Play();
            horn = gameObject.AddComponent<AudioSource>();
            horn.volume = 0.22f;
        }

        private void Update()
        {
            if (engine != null)
            {
                engine.pitch = Mathf.Lerp(0.75f, 1.8f, Mathf.Clamp01(controller.SpeedKph / 80f));
                engine.volume = Mathf.Lerp(0.07f, 0.18f, Mathf.Clamp01(controller.SpeedKph / 50f));
            }

            if (Input.GetKeyDown(KeyCode.H) && !horn.isPlaying)
            {
                horn.clip = CreateHornTone();
                horn.Play();
            }

            if (transform.position.y < -2f)
            {
                transform.position = GameBootstrap.MapPoint(new Vector2(36.81635f, -1.28755f)) + Vector3.up * 2f;
            }
        }

        private static AudioClip CreateEngineTone()
        {
            return CreateTone(48f, 1f, 0.18f);
        }

        private static AudioClip CreateHornTone()
        {
            return CreateTone(310f, 0.55f, 0.25f);
        }

        private static AudioClip CreateTone(float frequency, float duration, float gain)
        {
            var sampleRate = 44100;
            var data = new float[Mathf.CeilToInt(sampleRate * duration)];
            for (var i = 0; i < data.Length; i++)
            {
                var t = (float)i / sampleRate;
                var envelope = Mathf.Clamp01(1f - t / duration);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * gain * envelope;
            }
            var clip = AudioClip.Create("MatatuProceduralAudio", data.Length, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
