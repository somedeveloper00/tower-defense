using UnityEngine;

namespace TowerDefense.Input {
    public class MobilePointerDevice : PointerDevice {
        Vector2 _lastPos;
        bool _down = false;
        
        public override Vector2 GetPointerPos() => _lastPos;

        public override bool GetPointerDown() {
            if (UnityEngine.Input.touchCount > 0) {
                var touchPhase = UnityEngine.Input.GetTouch( 0 ).phase;
                return touchPhase == TouchPhase.Began;
            }
            return false;
        }

        public override bool GetPointerHeldDown() => _down;

        public override bool GetPointerUp() => UnityEngine.Input.touchCount > 0 &&
                                               UnityEngine.Input.GetTouch( 0 ).phase == TouchPhase.Ended;

        protected override bool IsSupported() => !Application.isEditor;

        public void Update() {
            if (UnityEngine.Input.touchCount > 0) {
                var touch = UnityEngine.Input.GetTouch( 0 );
                
                _lastPos = touch.position;

                if (touch.phase == TouchPhase.Began) _down = true;
                else if (touch.phase == TouchPhase.Ended) _down = false;
            }
        }


    }
}