using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/Preferences Exists")]
    public class TutorialCondition_Preferences_Exists : TutorialCondition {
        [SerializeField] string key;
        public bool exists;
        public override bool IsMet() => PreferencesDatabase.Current is not null && PreferencesDatabase.Current.KeyExists( key ) == exists;
    }
}