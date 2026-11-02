// One-shot batch setup for the Wave Keeper project (run once at creation by the Builder lane setup).
// Sets: 2D default behaviour mode, portrait 1080x1920, product "Wave Keeper", company "Captain",
// creates Assets/WaveKeeper/Scenes/Main.unity and Assets/WaveKeeper/Data, adds Main to Build Settings.
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

namespace WaveKeeper.EditorTools
{
    public static class WaveKeeperSetup
    {
        public static void Apply()
        {
            EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode2D;
            PlayerSettings.productName = "Wave Keeper";
            PlayerSettings.companyName = "Captain";
            PlayerSettings.defaultScreenWidth = 1080;
            PlayerSettings.defaultScreenHeight = 1920;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.captain.wavekeeper");
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            Directory.CreateDirectory("Assets/WaveKeeper/Scenes");
            Directory.CreateDirectory("Assets/WaveKeeper/Data");
            Directory.CreateDirectory("Assets/WaveKeeper/Scripts");
            AssetDatabase.Refresh();

            const string scenePath = "Assets/WaveKeeper/Scenes/Main.unity";
            if (!File.Exists(scenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                var cam = Camera.main;
                if (cam != null)
                {
                    cam.orthographic = true;
                    cam.orthographicSize = 9.6f; // 1920 px tall at 100 px per unit
                    cam.backgroundColor = new Color(0.07f, 0.08f, 0.12f);
                    cam.clearFlags = CameraClearFlags.SolidColor;
                }
                EditorSceneManager.SaveScene(scene, scenePath);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scenePath, true) };

            // Keep the Unity-made SampleScene out of the way if it exists.
            if (File.Exists("Assets/Scenes/SampleScene.unity"))
            {
                AssetDatabase.DeleteAsset("Assets/Scenes/SampleScene.unity");
                AssetDatabase.DeleteAsset("Assets/Scenes");
            }
            AssetDatabase.SaveAssets();
            Debug.Log("WaveKeeperSetup.Apply: done (2D, portrait 1080x1920, Wave Keeper / Captain, Main.unity)");
        }
    }
}
