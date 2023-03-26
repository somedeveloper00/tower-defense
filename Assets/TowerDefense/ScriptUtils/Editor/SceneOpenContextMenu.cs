using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TowerDefense.Editor {
    public class SceneOpenContextMenu {
        [MenuItem("Scenes/Main")]
        private static void OpenMainScene() {
            EditorSceneManager.OpenScene( "Assets/TowerDefense/Main/MainScene.unity" );
        }

        [MenuItem("Scenes/Env1")]
        private static void OpenEvn1() {
            EditorSceneManager.OpenScene( "Assets/TowerDefense/Core/Env/Env1/Env1.unity" );
        }
    }
}