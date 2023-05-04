using UnityEditor;

namespace TowerDefense.UI.Editor {
    [CustomEditor( typeof(DelayedButton), true )]
    public class DelayedButtonEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }
}