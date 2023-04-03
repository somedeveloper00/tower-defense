using UnityEngine;

namespace TowerDefense.Core.UI {
    public class CoreUI : MonoBehaviour {
        public static CoreUI Current;

        void OnEnable() => Current = this;
    }
}