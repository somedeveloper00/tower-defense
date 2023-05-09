using System.Threading.Tasks;
using AnimFlex.Core.Proxy;
using AnimFlex.Tweening;
using TowerDefense.Core.Audio;
using TowerDefense.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI {
    public abstract class DelayedButton : Button {
        public AudioClip clip;
        public float volume = 0.8f;
        public bool useDefaultAnim = true;
        public int delayAfterAnim = 10;

        public ButtonClickedEvent onClick { get; private set; } = new ButtonClickedEvent();

        bool _busy = false;
        
        protected override void Start() {
            base.Start();
            base.onClick.AddListener( _onClick );
        }

        async void _onClick() {
            if (_busy) return;
            _busy = true;
            
            enabled = false;
            bool finished = false;

            if (CoreAudioSource.Current) {
                CoreAudioSource.Current.audioSource.PlayOneShot( clip, volume );
            } else if (LobbyManager.Current) {
                LobbyManager.Current.generalAudioSource.PlayOneShot( clip, volume );
            }
            else {
                var source = GetComponentInParent<AudioSource>();
                if (source) {
                    source.PlayOneShot( clip, volume );
                }
            }

            if (useDefaultAnim) {
                transform.localScale = Vector3.one;
                transform.AnimScaleTo( Vector3.one * 0.7f, proxy: AnimFlexCoreProxyUnscaled.Default )
                    .SetDuration( 0.2f )
                    .SetEase( Ease.InOutCirc )
                    .SetPingPong()
                    .AddOnComplete( () => finished = true );
            }
            await PlayCustomAnim();

            await Task.Delay( 200 + delayAfterAnim );
            
            
            enabled = true;
            
            OnClick();
            onClick?.Invoke();
            _busy = false;
        }

        /// <summary> executes at the same time with the default animation </summary>
        protected virtual async Task PlayCustomAnim() => Task.Yield();

        protected abstract void OnClick();
    }
}