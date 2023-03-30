using System.Collections.Generic;
using System.Linq;
using TowerDefense.Core.Enemies;
using TowerDefense.Core.Defenders;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Road;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core {
    public class CoreGameManager : MonoBehaviour {
        public RoadManager roadManager;
        
        public float life = 20;
        
        [ShowInInspector, ReadOnly] public readonly List<Enemy> enemies = new (16);
        [ShowInInspector, ReadOnly] public readonly List<Defender> defenders = new (8);
        [ShowInInspector, ReadOnly] public readonly List<EnemySpawner> spawners = new(4);

        void Start() {
            CoreGameEvents.Current.OnEnemySpawn += OnEnemySpawn;
            CoreGameEvents.Current.OnEnemyReachEnd += OnEnemyReachEnd;
            CoreGameEvents.Current.OnEnemyDestroy += OnEnemyDestroy;
            CoreGameEvents.Current.OnDefenderDestroy += OnDefenderDestroy;
            CoreGameEvents.Current.OnDefenderSpawn += OnDefenderSpawn;
            CoreGameEvents.Current.OnSpawnerInitialize += OnSpawnerInitialize;
        }

        void OnSpawnerInitialize(EnemySpawner spawner) {
            spawners.Add( spawner );
        }

        void OnDefenderSpawn(Defender defender) {
            defender.coreGameManager = this;
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
                CoreGameEvents.Current.onWin?.Invoke();
            }
        }

        void OnEnemyReachEnd(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            life -= 1;
            if (life <= 0) {
                Debug.Log( $"Lost Game!" );
                CoreGameEvents.Current.onLose?.Invoke();
            }
        }
    }
}