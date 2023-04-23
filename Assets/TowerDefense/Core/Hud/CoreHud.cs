using System;
using UnityEngine;

namespace TowerDefense.Core.Hud {
    public class CoreHud : MonoBehaviour {
        public static CoreHud Instance;

        Camera _cam;

        protected void OnEnable() {
            if ( Instance != null ) {
                Debug.LogError( $"Already another instance. Destroying new one." );
                Destroy( Instance.gameObject );
            }
            Instance = this;
        }

        public Vector3 GetViewportPos(Vector3 worldPos) {
            if (!_cam) _cam = Camera.main;
            var p = (Vector2)_cam.WorldToViewportPoint( worldPos );
            var size = ((RectTransform)transform).sizeDelta;
            p.x = (p.x - 0.5f) * size.x;
            p.y = (p.y - 0.5f) * size.y;
            return p;
        }
    }
}