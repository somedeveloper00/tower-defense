using System;
using System.Threading.Tasks;
using AnimFlex.Sequencer;
using DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.DefenderSpawn {
    public class ConfirmationDialogue : Dialogue {
        [SerializeField] Button cancelBtn, okBtn;
        [SerializeField] SequenceAnim showSeq, hideSeq;

        int _playingAnims = 0;
        public event Action<bool> onResult;

        public async void Show() {
            await waitForOtherAnims();
            showSeq.PlaySequence();
        }
        
        public async void Hide() {
            await waitForOtherAnims();
            hideSeq.PlaySequence();
        }

        async Task waitForOtherAnims() {
            if (showSeq.sequence.IsPlaying()) showSeq.sequence.Stop();
            if (hideSeq.sequence.IsPlaying()) hideSeq.sequence.Stop();
            while (_playingAnims > 0) {
                await Task.Yield();
            }
        }
        

        protected override void Start() {
            base.Start();

            void startAnim() => canvasRaycaster.enabled = false;
            void endAnim() => canvasRaycaster.enabled = true;

            showSeq.sequence.onPlay += startAnim;
            showSeq.sequence.onComplete += endAnim;
            hideSeq.sequence.onPlay += startAnim;
            hideSeq.sequence.onComplete += endAnim;
            
            cancelBtn.onClick.AddListener( () => {
                onResult?.Invoke( false );
            } );
            
            okBtn.onClick.AddListener( () => {
                onResult?.Invoke( true );
            } );
        }
    }
}