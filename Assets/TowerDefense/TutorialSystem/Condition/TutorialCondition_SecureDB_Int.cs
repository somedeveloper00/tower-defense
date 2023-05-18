using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/SecureDB Int")]
    public class TutorialCondition_SecureDB_Int : TutorialCondition {
        [SerializeField] string key;
        [SerializeField] ConditionStatement @is;
        [SerializeField] int value;

        public override bool IsMet() {
            if (SecureDatabase.Current is not null && SecureDatabase.Current.GetInt( key, out var v )) {
                return @is switch {
                    ConditionStatement.EqualTo => v == value,
                    ConditionStatement.GreaterThan => v > value,
                    ConditionStatement.LessThan => v < value,
                    ConditionStatement.GreaterThanOrEqualTo => v >= value,
                    ConditionStatement.LessThanOrEqualTo => v <= value,
                    _ => false
                };
            }

            return false;
        }

    }
}