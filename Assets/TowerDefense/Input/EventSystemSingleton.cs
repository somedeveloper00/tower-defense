using UnityEngine.EventSystems;

namespace TowerDefense.Input {
    public class EventSystemSingleton : EventSystem {
        static EventSystemSingleton current;
        EventSystem _eventSystem;

        protected override void Start() {
            _eventSystem = GetComponent<EventSystem>();
            if (current == null) {
                current = this;
            }
            else if (current != this) {
                foreach (var pointerInputModule in GetComponents<PointerInputModule>())
                    Destroy( pointerInputModule );
                Destroy( _eventSystem );
            }
            base.Start();
        }
    }
}