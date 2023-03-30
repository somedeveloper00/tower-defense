using UnityEngine;

namespace TowerDefense.Input {
    public class PcPointerDevice : PointerDevice {
        public override Vector2 GetPointerPos() => UnityEngine.Input.mousePosition;
        public override bool GetPointerDown() => UnityEngine.Input.GetMouseButtonDown( 0 );
        public override bool GetPointerHeldDown() => UnityEngine.Input.GetMouseButton( 0 );
        public override bool GetPointerUp() => UnityEngine.Input.GetMouseButtonUp( 0 );
        protected override bool IsSupported() => Application.isEditor;
    }
}