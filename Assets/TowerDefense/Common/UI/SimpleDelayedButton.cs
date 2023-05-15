using System.Threading.Tasks;
using AnimFlex.Sequencer;

namespace TowerDefense.UI {
    public class SimpleDelayedButton : DelayedButton {
        public SequenceAnim onClipSeq;

        protected override async Task PlayCustomAnim() {
            if (onClipSeq) {
                onClipSeq.PlaySequence();
                await onClipSeq.AwaitComplete();
            }
        }

        protected override void OnClick() { }
    }
}