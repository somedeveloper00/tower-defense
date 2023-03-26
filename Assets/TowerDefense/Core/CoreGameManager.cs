using System.Collections.Generic;
using System.Linq;
using TowerDefense.Core.Enemies;
using TowerDefense.Core.Defenders;
using TowerDefense.Core.Road;
using TowerDefense.Core.Spawn;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core {
    public class CoreGameManager : MonoBehaviour {
        public RoadManager roadManager;
        public CoreGameEvents coreGameEvents;
        
        public float life = 20;
        
        [ShowInInspector, ReadOnly] public readonly List<Enemy> enemies = new (16);
        [ShowInInspector, ReadOnly] public readonly List<Defender> defenders = new (8);
        [ShowInInspector, ReadOnly] public readonly List<EnemySpawner> spawners = new(4);

        void Awake() {
            coreGameEvents.OnEnemySpawn += OnEnemySpawn;
            coreGameEvents.OnEnemyReachEnd += OnEnemyReachEnd;
            coreGameEvents.OnEnemyDestroy += OnEnemyDestroy;
            coreGameEvents.OnDefenderDestroy += OnDefenderDestroy;
            coreGameEvents.OnDefenderSpawn += OnDefenderSpawn;
            coreGameEvents.OnSpawnerInitialize += OnSpawnerInitialize;
        }

        void OnSpawnerInitialize(EnemySpawner spawner) {
            spawners.Add( spawner );
        }

        void OnDefenderSpawn(Defender defender) {
            defenders.Add( defender );
        }
        void OnDefenderDestroy(Defender defender) {
            Destroy( defender.gameObject );
        }
            
        void OnEnemySpawn(Enemy enemy) {
            enemies.Add( enemy );
        }

        void OnEnemyDestroy(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            // check if any spawners has any enemy to spawn
            if (enemies.Count == 0 && spawners.All( s => s.IsDone() )) {
                Debug.Log( $"Won Game!" );
                coreGameEvents.onWin?.Invoke();
            }
        }

        void OnEnemyReachEnd(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            life -= 1;
            if (life <= 0) {
                Debug.Log( $"Lost Game!" );
                coreGameEvents.onLose?.Invoke();
            }
        }
    }
}