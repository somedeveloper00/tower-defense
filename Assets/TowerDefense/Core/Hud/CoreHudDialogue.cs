using DialogueSystem;
using UnityEngine;

namespace TowerDefense.Core.Hud {
    public class CoreHudDialogue : Dialogue {
        public static CoreHudDialogue Instance;

        protected override void OnEnable() {
            base.OnEnable();
            if ( Instance != null ) {
                Debug.LogError( $"Already another instance. Destroying new one." );
                Destroy( Instance.gameObject );
            }
            Instance = this;
        }
    }
}