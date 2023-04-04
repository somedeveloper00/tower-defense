using System;
using TowerDefense.Core.Enemies;
using TowerDefense.Core.Defenders;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Starter;
using UnityEngine;

namespace TowerDefense.Core {
    [CreateAssetMenu( fileName = "CoreGameEvents", menuName = "Core/Game Events", order = 0 )]
    public class CoreGameEvents : ScriptableObject {
        public Action<CoreGameManager> OnGameStart;
        public Action<CoreLevelData> OnStartupFinished;
        public Action<EnemySpawner> OnEnemySpawnerInitialize;
        public Action<Enemy> OnEnemySpawn;
        public Action<Enemy> OnEnemyDestroy;
        public Action<Defender> OnDefenderSpawn;
        public Action<Defender> OnDefenderDestroy;
        public Action<Enemy> OnEnemyReachEnd;
        public Action onLose;
        public Action onWin;

        public static CoreGameEvents Current;

        void OnEnable() => Current = this;
    }
}