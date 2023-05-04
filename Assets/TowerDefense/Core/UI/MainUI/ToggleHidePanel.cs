using System.Threading.Tasks;
using AnimFlex.Sequencer.UserEnd;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI {
    [RequireComponent(typeof(Button))]
    public class ToggleHidePanel : MonoBehaviour {

        [PropertyTooltip("in milliseconds")]
        [SerializeField] int delay = 300;
        [SerializeField] SequenceAnim hideSeq, showSeq;
        
        Button _button;
        bool _hidden = false;
        int _lock = 0;
        
        void Start() {
            _button = GetComponent<Button>();
            _button.onClick.AddListener( ToggleHide );
            hideSeq.sequence.onPlay += onSeqPlay;
            showSeq.sequence.onPlay += onSeqPlay;
            hideSeq.sequence.onComplete += onSeqComplete;
            showSeq.sequence.onComplete += onSeqComplete;

            void onSeqComplete() {
                _lock--;
                _button.targetGraphic.raycastTarget = _lock == 0;
            }

            void onSeqPlay() {
                _button.targetGraphic.raycastTarget = false;
                _lock++;
            }
        }

        async void ToggleHide() {
            if (_lock > 0) return;
            _lock++;
            await Task.Delay( delay );
            _lock--;
            if (_hidden) showSeq.PlaySequence();
            else hideSeq.PlaySequence();
            _hidden = !_hidden;
        }
    }
}