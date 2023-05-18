using UnityEngine;

namespace TowerDefense.TutorialSystem {
    public abstract class TutorialCondition : ScriptableObject {
        public abstract bool IsMet();
    }
}