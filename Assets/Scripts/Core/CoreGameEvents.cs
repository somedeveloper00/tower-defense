using System;
using Core.Defenders;
using Core.Enemies;
using UnityEngine;

namespace Core {
    [CreateAssetMenu( fileName = "CoreGameEvents", menuName = "Core/Game Events", order = 0 )]
    public class CoreGameEvents : ScriptableObject {
        public Action<GameManager> OnGameStart;
        public Action<Enemy> OnEnemySpawn;
        public Action<Enemy> OnEnemyDestroy;
        public Action<Defender> OnDefenderSpawn;
        public Action<Defender> OnDefenderDestroy;
        public Action<Enemy> OnEnemyReachEnd;
    }
}