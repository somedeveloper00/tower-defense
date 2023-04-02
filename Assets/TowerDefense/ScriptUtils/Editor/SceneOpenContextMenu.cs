using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace TowerDefense.Editor {
    public static class SceneOpenContextMenu {
        [MenuItem("Scenes/Lobby")]
        private static void OpenLobbyScene() {
            EditorSceneManager.OpenScene( "Assets/TowerDefense/Lobby/Lobby.unity" );
        }

        [MenuItem("Scenes/Env1")]
        private static void OpenEvn1() {
            EditorSceneManager.OpenScene( "Assets/TowerDefense/Core/Env/Env1/Env1.unity" );
        }
    }
}