using UnityEngine;

namespace TowerDefense.Core.Hud {
    public class CoreHud : MonoBehaviour {
        public static CoreHud Instance;

        protected void OnEnable() {
            if ( Instance != null ) {
                Debug.LogError( $"Already another instance. Destroying new one." );
                Destroy( Instance.gameObject );
            }
            Instance = this;
        }
    }
}