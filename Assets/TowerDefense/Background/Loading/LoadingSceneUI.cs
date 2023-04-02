using AnimFlex.Sequencer.UserEnd;
using UnityEngine;

namespace TowerDefense.Background.Loading {
    public class LoadingSceneUI : MonoBehaviour {
        public Canvas canvas;
        public SequenceAnim inSequence, outSequence;

        public void DestroyGameObject() {
            Destroy( gameObject );
        }
    }
}