using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.DevelopmentOnly {
    [RequireComponent(typeof(Text))]
    [ExecuteAlways]
    public class IngameTimeText : MonoBehaviour {
        Text txt;
        void Start() => txt = GetComponent<Text>();
        void LateUpdate() => txt.text = Time.unscaledTime.ToString("#,0");
    }
}