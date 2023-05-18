using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Lose In Row")]
    public class TutorialCondition_LoseInRow : TutorialCondition {
        [SerializeField] int count;
        [SerializeField] ConditionStatement condition;
        int c = 0;

        void OnEnable() {
            c = count;
            CoreGameEvents.Current.onLose += (_) => c++;
            CoreGameEvents.Current.onWin += (_) => c = 0;
        }

        public override bool IsMet() => condition.Matches( c, count );
    }
}