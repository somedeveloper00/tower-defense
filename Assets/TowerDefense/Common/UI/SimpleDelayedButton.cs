using System.Threading.Tasks;
using AnimFlex.Sequencer;

namespace TowerDefense.UI {
    public class SimpleDelayedButton : DelayedButton {
        public SequenceAnim onClipSeq;

        protected override Task PlayCustomAnim() {
            if (onClipSeq) {
                onClipSeq.PlaySequence();
                return onClipSeq.AwaitComplete();
            }
            return Task.CompletedTask;
        }

        protected override void OnClick() { }
    }
}