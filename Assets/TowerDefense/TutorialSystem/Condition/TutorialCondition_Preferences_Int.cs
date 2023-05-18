using System;
using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Preferences Int")]
    public class TutorialCondition_Preferences_Int : TutorialCondition {
        [SerializeField] string key;
        [SerializeField] ConditionStatement @is;
        [SerializeField] int value;

        public override bool IsMet() {
            if (PreferencesDatabase.Current is not null && PreferencesDatabase.Current.GetInt( key, out var v ))
                return @is.Matches( v, value );

            return false;
        }

    }
}