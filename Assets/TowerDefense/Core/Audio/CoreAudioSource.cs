using UnityEngine;

namespace TowerDefense.Core.Audio {
    [RequireComponent(typeof(AudioSource))]
    public class CoreAudioSource : MonoBehaviour {
        public AudioSource audioSource;
        public static CoreAudioSource Current;
        void OnEnable() => Current = this;
        void OnValidate() => audioSource = GetComponent<AudioSource>();
    }
}