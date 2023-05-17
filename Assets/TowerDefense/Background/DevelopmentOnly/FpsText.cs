using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.DevelopmentOnly {
    [RequireComponent(typeof(Text))]
    public class FpsText : MonoBehaviour {
        Text txt;

        void Start() => txt = GetComponent<Text>();
        void Update() {
            txt.text = $"{Time.unscaledDeltaTime:0.00}ms - {(int)(1f / Time.unscaledDeltaTime)} FPS";
        }
    }
}