using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Preferences Int")]
    public class TutorialCondition_Preferences_Int : TutorialCondition {
        [SerializeField] string key;
        [SerializeField] ConditionStatement @is;
        [SerializeField] int value;

        public override bool IsMet() {
            if (PreferencesDatabase.Current is not null && PreferencesDatabase.Current.GetInt( key, out var v )) {
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
    enum ConditionStatement {
        EqualTo, GreaterThan, LessThan, GreaterThanOrEqualTo, LessThanOrEqualTo
    }
}