using System;
using TowerDefense.Core.UI.Lose;
using TowerDefense.Core.UI.Win;
using UnityEngine;

namespace TowerDefense.Core.Defenders {
    [CreateAssetMenu( fileName = "DefenderSpawnStats", menuName = "TD/Core/Defender Spawn Stats", order = 0 )]
    public class DefenderSpawnStats : ScriptableObject {
        public long initialCost;
        public int costIncrement;

        [NonSerialized] int _spawned;
        
        public void ResetCosts() {
            Debug.Log( $"costs reset for {name}" );
            _spawned = 0;
        }

        public void IncreaseCost() => _spawned++;
        public long GetCurrentCost() => initialCost + _spawned * costIncrement;

        void OnEnable() {
            CoreGameEvents.Current.onLose += onLose;
            CoreGameEvents.Current.onWin += onWin;
            CoreGameEvents.Current.OnGameStart += onStart;
        }
        
        void OnDisable() {
            CoreGameEvents.Current.onLose -= onLose;
            CoreGameEvents.Current.onWin -= onWin;
            CoreGameEvents.Current.OnGameStart -= onStart;
        }

        void onStart(CoreGameManager _) => ResetCosts();
        void onLose(LoseData _) => ResetCosts();
        void onWin(WinData _) => ResetCosts();
    }
}