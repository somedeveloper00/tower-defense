using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.Data {
    public class PreferencesDatabaseKeyCounterIncrease : MonoBehaviour {
        [SerializeField] string fullKey;

        void Start() {
            var count = PreferencesDatabase.Current.GetInt( fullKey, out var v ) ? v : 0;
            PreferencesDatabase.Current.Set( fullKey, count + 1 );
        }
    }
}