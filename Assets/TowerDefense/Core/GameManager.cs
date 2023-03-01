using System;
using System.Collections.Generic;
using Core.Defenders;
using Core.Enemies;
using TriInspector;
using UnityEngine;

namespace Core {
    public class GameManager : MonoBehaviour {
        public RoadManager roadManager;
        public CoreGameEvents coreGameEvents;
        
        public float life;

        [ShowInInspector, ReadOnly] public readonly List<Enemy> enemies = new (16);
        [ShowInInspector, ReadOnly] public readonly List<Defender> defenders = new (8);

        void Awake() {
            coreGameEvents.OnEnemySpawn += OnEnemySpawn;
            coreGameEvents.OnEnemyReachEnd += OnEnemyReachEnd;
            coreGameEvents.OnEnemyDestroy += OnEnemyDestroy;
            coreGameEvents.OnDefenderDestroy += OnDefenderDestroy;
            coreGameEvents.OnDefenderSpawn += OnDefenderSpawn;
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
        }

        void OnEnemyReachEnd(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            life -= 1;
        }
    }
}