using AnimFlex.Sequencer.UserEnd;
using TowerDefense.UI;
using UnityEngine;

namespace TowerDefense.Core.UI {
    public class ToggleHidePanel : DelayedButton {

        [SerializeField] SequenceAnim hideSeq, showSeq;
        
        bool _hidden = false;
        int _lock = 0;

        protected override void Start() {
            base.Start();
            hideSeq.sequence.onPlay += onSeqPlay;
            showSeq.sequence.onPlay += onSeqPlay;
            hideSeq.sequence.onComplete += onSeqComplete;
            showSeq.sequence.onComplete += onSeqComplete;

            void onSeqComplete() {
                _lock--;
                enabled = _lock == 0;
            }

            void onSeqPlay() {
                enabled = false;
                _lock++;
            }
        }

        protected override async void OnClick() {
            if (_lock > 0) return;
            if (_hidden) showSeq.PlaySequence();
            else hideSeq.PlaySequence();
            _hidden = !_hidden;
        }
    }
}