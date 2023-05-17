using UnityEngine;

namespace TowerDefense.UI {
    [RequireComponent(typeof(Camera))]
    public class UiCamera : MonoBehaviour {
        public new Camera Camera { get; private set; }
        public static UiCamera Current { get; private set; }
        void Start() {
            Camera = GetComponent<Camera>();
            Current = this;
        }
    }
}