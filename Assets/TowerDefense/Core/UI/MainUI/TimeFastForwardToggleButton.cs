using RTLTMPro;
using TowerDefense.UI;
using UnityEngine;

namespace TowerDefense.Core.UI {
    public class TimeFastForwardToggleButton : DelayedButton {
        [SerializeField] float fastForwardSpeed = 2f;
        [SerializeField] RTLTextMeshPro text;
        [SerializeField] string fastForwardText = ">>";
        [SerializeField] string normalText = ">";

        bool _isFastForward;

        protected override void OnClick() {
            if (_isFastForward) Time.timeScale = 1;
            else Time.timeScale = fastForwardSpeed;
            _isFastForward = !_isFastForward;
            text.text = _isFastForward ? fastForwardText : normalText;
        }
    }
}