using TowerDefense.Core.Audio;
using UnityEngine;

namespace TowerDefense.Core.Enemies.SimpleBalloon {
    public class SimpleBalloonEnemy : Enemy {

        [SerializeField] AudioClip onDestroyAudio;
        [SerializeField] float onDestroyAudioVolume;

        protected override void destroy() {
            base.destroy();
            CoreAudioSource.Current.audioSource.PlayOneShot( onDestroyAudio, onDestroyAudioVolume );
        }
    }
}