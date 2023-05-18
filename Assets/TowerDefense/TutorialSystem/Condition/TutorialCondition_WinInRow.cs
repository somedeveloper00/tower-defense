using TowerDefense.Core;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Win In Row")]
    public class TutorialCondition_WinlInRow : TutorialCondition {
        [SerializeField] int count;
        [SerializeField] ConditionStatement conditionStatement;
        int c = 0;

        void OnEnable() {
            c = count;
            CoreGameEvents.Current.onWin += (_) => c++;
            CoreGameEvents.Current.onLose += (_) => c = 0;
        }

        public override bool IsMet() => conditionStatement.Matches( c, count );
    }
}