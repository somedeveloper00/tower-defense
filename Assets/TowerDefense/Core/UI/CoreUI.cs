using AnimFlex.Core.Proxy;
using AnimFlex.Tweening;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.UI {
    public class CoreUI : MonoBehaviour {
        public static CoreUI Current;
        public GameObject[] objbj;
        public bool tt = false;

        void OnEnable() => Current = this;

        [Button]
        void ono() {
            foreach (var o in objbj) {
                o.SetActive( true );
            }

            if (tt)
            Tweener.Generate(
                () => Time.timeScale, value => Time.timeScale = value, 0, 0.2f, proxy: AnimFlexCoreProxyUnscaled.Default );
        }
    }
}