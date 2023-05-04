using System.Threading.Tasks;
using AnimFlex;
using AnimFlex.Sequencer.UserEnd;
using AnimFlex.Tweening;
using UnityEngine;

namespace TowerDefense.Core.UI {
    public class ToggleMuteButton : CoreDelayedButton {
        
        [SerializeField] AudioSource musicAudioSource;
        [SerializeField] float volumeChangeDuration = 0.5f;
        [SerializeField] SequenceAnim muteSeq, unmuteSeq;
        
        bool _muted = false;
        float _vol;
        Tweener _tweener;

        protected override void Start() {
            base.Start();
            _vol = musicAudioSource.volume;
        }

        protected override async Task PlayCustomAnim() {
            if (_muted) {
                unmuteSeq.PlaySequence();
                await unmuteSeq.sequence.AwaitComplete();
            }
            else {
                muteSeq.PlaySequence();
                await muteSeq.sequence.AwaitComplete();
            }
        }

        protected override void OnClick() {
            if (_tweener is not null && _tweener.IsActive()) _tweener.Kill( false, false );
            if (_muted) _tweener = musicAudioSource.AnimAudioVolumeTo( _vol, duration: volumeChangeDuration );
            else _tweener = musicAudioSource.AnimAudioVolumeTo( 0, duration: volumeChangeDuration );
            _muted = !_muted;
        }
    }
}