using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Core.Enemies;
using UnityEngine;

namespace TowerDefense.Core.EnemySpawn {
    [CreateAssetMenu( fileName = "EnemySpawnerDatabasse", menuName = "Core/Enemy Spawner Database", order = 0 )]
    public class EnemyDatabase : ScriptableObject {

        public static EnemyDatabase Current;
        void OnEnable() => Current = this;

        [SerializeField] List<EnemyType> enemySpawnTypes;

        public Enemy GetEnemyPrefab(string name) => getEnemyType( name ).prefab;
        public int GetEnemyWorth(string name) => getEnemyType( name ).worth;

        public string[] GetAllNames() => enemySpawnTypes.Select( e => e.name ).ToArray();

        Dictionary<string, EnemyType> cache = new();
        
        private EnemyType getEnemyType(string name) {
            if (cache.TryGetValue( name, out var e )) return e;
            foreach (var enemySpawnType in enemySpawnTypes)
                if (enemySpawnType.prefab != null && enemySpawnType.name == name) {
                    cache.Add( name, enemySpawnType );
                    return enemySpawnType;
                }
            throw new Exception( $"enemy not found: {name}" );
        }

        [Serializable]
        public class EnemyType {
            public string name;
            public Enemy prefab;
            public int worth;
        }
    }
}