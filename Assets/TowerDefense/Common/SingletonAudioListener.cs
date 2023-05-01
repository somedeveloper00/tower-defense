using UnityEngine;

namespace TowerDefense.Common {
    [RequireComponent(typeof(AudioListener))]
    public class SingletonAudioListener : MonoBehaviour {
        public AudioListener audioListener;

        void OnValidate() {
            audioListener = GetComponent<AudioListener>();
        }

        public static SingletonAudioListener Current;

        void OnEnable() {
            if (Current) {
                Destroy( gameObject );
                return;
            }
            Current = this;
        }
    }
}