using TowerDefense.Data.Database;
using UnityEngine;

namespace TowerDefense.Data {
    public class SecureDatabaseKeyCounterIncrease : MonoBehaviour {
        [SerializeField] string fullKey;

        void Start() {
            var count = SecureDatabase.Current.GetInt( fullKey, out var v ) ? v : 0;
            SecureDatabase.Current.Set( fullKey, count + 1 );
        }
    }
}