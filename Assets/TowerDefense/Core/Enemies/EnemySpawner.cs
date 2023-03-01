using UnityEngine;

namespace Core.Enemies {
    [CreateAssetMenu( fileName = "EnemySpawner", menuName = "Core/Enemy Spawner", order = 0 )]
    public class EnemySpawner : ScriptableObject {
        [SerializeField] Enemy enemyPrefab;

        public Enemy Spawn(Transform parent) {
            var enemy = Instantiate( enemyPrefab, parent );
            return enemy;
        }
    }
}