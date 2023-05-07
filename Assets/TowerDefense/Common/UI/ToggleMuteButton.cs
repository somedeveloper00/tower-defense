using System.Threading.Tasks;
using AnimFlex.Sequencer.UserEnd;
using TowerDefense.Music;
using UnityEngine;

namespace TowerDefense.UI {
    public class ToggleMuteButton : DelayedButton {
        [SerializeField] MusicPlayer player;
        [SerializeField] SequenceAnim muteSeq, unmuteSeq;

        protected override void Awake() {
            base.Awake();
            player.onLoaded += () => {
                if (player.IsMuted()) {
                    muteSeq.PlaySequence();
                }
            };
        }

        protected override async Task PlayCustomAnim() {
            if (player.IsMuted()) {
                player.Unmute( true );
                unmuteSeq.PlaySequence();
                await unmuteSeq.sequence.AwaitComplete();
            }
            else {
                player.Mute( true );
                muteSeq.PlaySequence();
                await muteSeq.sequence.AwaitComplete();
            }
        }

        protected override void OnClick() { }
    }
}