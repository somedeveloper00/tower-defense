using System;
using System.Collections.Generic;
using System.Linq;
using TowerDefense.Core.Enemies;
using UnityEngine;

namespace TowerDefense.Core.Spawn {
    [CreateAssetMenu( fileName = "EnemySpawnerDatabasse", menuName = "Core/Enemy Spawner Database", order = 0 )]
    public class EnemyDatabase : ScriptableObject {

        public static EnemyDatabase Current;
        void OnEnable() => Current = this;

        [SerializeField] List<EnemyType> enemySpawnTypes;

        public Enemy GetEnemyPrefab(string name) {
            foreach (var enemySpawnType in enemySpawnTypes) {
                if ( enemySpawnType.prefab != null && enemySpawnType.name == name )
                    return enemySpawnType.prefab;
            }
            throw new Exception( "enemy of name not found: " + name );
        }

        public string[] GetAllNames() => enemySpawnTypes.Select( e => e.name ).ToArray();

        [Serializable]
        public class EnemyType {
            public string name;
            public Enemy prefab;
        }
    }
}