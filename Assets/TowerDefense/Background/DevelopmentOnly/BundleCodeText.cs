using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.DevelopmentOnly {
    [RequireComponent(typeof(Text))]
    [ExecuteAlways]
    public class BundleCodeText : MonoBehaviour {
        Text txt;

        void Start() {
            txt = GetComponent<Text>();
            txt.text = Application.version;
        }
    }
}