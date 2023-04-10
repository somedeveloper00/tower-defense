#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Common {
    [RequireComponent(typeof(Button))]
    public class PauseOnClick : MonoBehaviour {
        
        void OnEnable() {
            GetComponent<Button>().onClick.AddListener( Pause );
        }

        void OnDisable() {
            GetComponent<Button>().onClick.AddListener( Pause );
        }

        void Pause() => EditorApplication.isPaused = true;
    }
}
#endif