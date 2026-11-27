using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NairobiMatatu.Editor
{
    public static class NairobiMatatuBuilder
    {
        [MenuItem("Nairobi Matatu/Open Playable Scene")]
        public static void OpenScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/NairobiMatatu.unity");
        }

        [MenuItem("Nairobi Matatu/Build Windows")]
        public static void BuildWindows()
        {
            var scene = "Assets/Scenes/NairobiMatatu.unity";
            EditorSceneManager.OpenScene(scene);
            var options = new BuildPlayerOptions
            {
                scenes = new[] { scene },
                locationPathName = "Builds/Windows/NairobiMatatu.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            BuildPipeline.BuildPlayer(options);
        }
    }
}
