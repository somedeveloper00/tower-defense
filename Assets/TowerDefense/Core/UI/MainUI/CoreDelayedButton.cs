using System.Threading.Tasks;
using AnimFlex.Core.Proxy;
using AnimFlex.Tweening;
using TowerDefense.Core.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI {
    [RequireComponent(typeof(Button))]
    public abstract class CoreDelayedButton : MonoBehaviour {
        public AudioClip clip;
        public float volume = 0.8f;
        public bool useDefaultAnim = true;
        public int delayAfterAnim = 10;
        
        protected Button button { get; private set; }

        protected virtual void Start() {
            button = GetComponent<Button>();
            button.onClick.AddListener( onClick );
        }

        async void onClick() {
            button.enabled = false;
            bool finished = false;

            CoreAudioSource.Current.audioSource.PlayOneShot( clip, volume );
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
            
            
            button.enabled = true;
            
            OnClick();
        }

        /// <summary> executes at the same time with the default animation </summary>
        protected virtual async Task PlayCustomAnim() => Task.Yield();

        protected abstract void OnClick();
    }
}