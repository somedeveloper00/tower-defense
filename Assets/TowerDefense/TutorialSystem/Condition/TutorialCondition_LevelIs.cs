using TowerDefense.Data;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Level Is")]
    public class TutorialCondition_LevelIs : TutorialCondition {
        [SerializeField] string levelId;
        public override bool IsMet() => PlayerGlobals.Current && PlayerGlobals.Current.GetLastLevelId() == levelId;
    }
}