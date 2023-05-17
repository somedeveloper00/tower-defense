using System;
using TowerDefense.Core.Enemies;
using TowerDefense.Core.Defenders;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Starter;
using TowerDefense.Core.UI.Lose;
using TowerDefense.Core.UI.Win;
using TowerDefense.Data;
using UnityEngine;

namespace TowerDefense.Core {
    /// <summary>
    /// singleton class that holds all core game events
    /// </summary>
    public class CoreGameEvents : ScriptableObject {
        public Action<CoreGameManager> OnGameStart;
        public StartupFinished OnStartupFinished;
        public Action onSessionCoinModified;
        public Action onLifeModified;
        public Action onTimeModified;
        public Action<EnemySpawner> OnEnemySpawnerInitialize;
        public Action<Enemy> OnEnemySpawn;
        public Action<Enemy> OnEnemyDestroy;
        public Action<Defender> OnDefenderSpawn;
        public Action<Defender> OnDefenderDestroy;
        public Action<Enemy> OnEnemyReachEnd;
        public Action<LoseData> onLose;
        public Action<WinData> onWin;

        public static CoreGameEvents Current;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
#endif
        static void initialize() => Current = CreateInstance<CoreGameEvents>();

        
        public delegate void StartupFinished(CoreLevelData levelData, CoreSessionPack sessionPack);
    }
}