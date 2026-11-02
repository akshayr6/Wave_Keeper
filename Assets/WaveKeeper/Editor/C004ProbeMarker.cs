// C004 batch scene builder: adds one empty GameObject named "ProbeMarker" to the Main scene and nothing else.
// Idempotent: if ProbeMarker already exists at the scene root, the scene is left unchanged.
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace WaveKeeper.EditorTools
{
    public static class C004ProbeMarker
    {
        const string ScenePath = "Assets/WaveKeeper/Scenes/Main.unity";
        const string MarkerName = "ProbeMarker";

        public static void Apply()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == MarkerName)
                {
                    Debug.Log("C004: ProbeMarker already present, scene unchanged");
                    return;
                }
            }

            var marker = new GameObject(MarkerName);
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(marker, scene);
            EditorSceneManager.MarkSceneDirty(scene);
            if (!EditorSceneManager.SaveScene(scene))
                throw new System.Exception("C004: saving Main.unity failed");
            Debug.Log("C004: ProbeMarker added to Main.unity");
        }
    }
}
