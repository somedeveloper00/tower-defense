using UnityEngine;

namespace TowerDefense.Core.Hud {
    public class Hud3DToViewport : MonoBehaviour {
        public Transform hudTargetTransform;
        public RectTransform uiItem;
        public Vector2 offset;
        public GameObject[] deleteAfterReparent;

        void OnEnable() => uiItem.gameObject.SetActive( true );
        void OnDisable() => uiItem.gameObject.SetActive( false );
        void Start() {
            // put under core game's hud
            uiItem.SetParent( CoreHudDialogue.Instance.transform, false );
            foreach (var go in deleteAfterReparent) Destroy( go );
        }

        void LateUpdate() {
            updateHudPos();
        }

        void updateHudPos() {
            var p = (Vector2)Camera.main.WorldToViewportPoint( hudTargetTransform.position );
            var size = ((RectTransform)CoreHudDialogue.Instance.transform).sizeDelta;
            p.x = (p.x - 0.5f) * size.x;
            p.y = (p.y - 0.5f) * size.y;
            uiItem.anchoredPosition = p + offset;
        }
    }
}