using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.TutorialSystem {
    [CreateAssetMenu(menuName = "TD/Tutorial Conditions/SecureDB Exists")]
    public class TutorialCondition_SecureDB_Exists : TutorialCondition {
        [SerializeField] string key;
        public bool exists;
        public override bool IsMet() => SecureDatabase.Current is not null && SecureDatabase.Current.KeyExists( key ) == exists;
    }
}