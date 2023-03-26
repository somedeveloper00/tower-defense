using System;
using TowerDefense.Core.Enemies;
using TowerDefense.Core.Defenders;
using TowerDefense.Core.Spawn;
using UnityEngine;

namespace TowerDefense.Core {
    [CreateAssetMenu( fileName = "CoreGameEvents", menuName = "Core/Game Events", order = 0 )]
    public class CoreGameEvents : ScriptableObject {
        public Action<CoreGameManager> OnGameStart;
        public Action<EnemySpawner> OnSpawnerInitialize;
        public Action<Enemy> OnEnemySpawn;
        public Action<Enemy> OnEnemyDestroy;
        public Action<Defender> OnDefenderSpawn;
        public Action<Defender> OnDefenderDestroy;
        public Action<Enemy> OnEnemyReachEnd;
        public Action onLose;
        public Action onWin;
    }
}