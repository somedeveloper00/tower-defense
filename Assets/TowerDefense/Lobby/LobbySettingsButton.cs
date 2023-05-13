using AnimFlex.Sequencer;
using TowerDefense.UI;
using UnityEngine;

namespace TowerDefense.Lobby {
    public class LobbySettingsButton : DelayedButton {

        [SerializeField] SequenceAnim inSeq, outSeq;
        [SerializeField] CanvasGroup pannelCanvasGroup;

        bool _isVisible = false;
        
        protected override async void OnClick() {
            enabled = pannelCanvasGroup.blocksRaycasts = false;
            if (!_isVisible) {
                inSeq.PlaySequence();
                await inSeq.sequence.AwaitComplete();
            }
            else {
                outSeq.PlaySequence();
                await outSeq.sequence.AwaitComplete();
            }
            _isVisible = !_isVisible;
            enabled = pannelCanvasGroup.blocksRaycasts = true;
        }
    }
}