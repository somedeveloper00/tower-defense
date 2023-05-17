using System;
using System.Threading.Tasks;
using TowerDefense.Core.Defenders;
using TowerDefense.Core.Hud;
using UnityEngine;

namespace TowerDefense.Core.DefenderSpawn {
    public class DefenderSpawner : MonoBehaviour {

        public SpawnPositionSelector spawnPositionSelector;
        public ConfirmationDialogue confirmDialogue;

        public static DefenderSpawner Current;

        void OnEnable() => Current = this;

        void Start() {
            confirmDialogue.Hide();
        }

        public async Task SpawnUsingSelector(string defenderName) {
            var placeholder = DefenderDatabase.Current.GetDefenderPlaceholderPrefab( defenderName );
            spawnPositionSelector.gameObject.SetActive( true );
            spawnPositionSelector.SetPlaceholder( placeholder );
            bool finished = false;
            Vector3 lastPos = Vector3.zero;

            void OnConfirmDialogueOnonResult(bool result) {
                if (result) {
                    // confirm
                    var prefab = DefenderDatabase.Current.GetDefenderMainPrefab( defenderName );
                    var spawnStats = DefenderDatabase.Current.GetDefenderSpawnStats( defenderName );
                    SpawnDefender( prefab, spawnStats, lastPos );
                }

                confirmDialogue.Hide();
                finished = true;
            }

            void OnSpawnPositionSelectorOnOnSettle(Vector3 pos) {
                lastPos = pos;
                // show confirmation dialogue
                var pos2D = CoreHud.Current.GetViewportPos( pos );
                confirmDialogue.transform.localPosition = pos2D;
                confirmDialogue.Show();
            }

            void OnSpawnPositionSelectorOnOnStartDrag() {
                confirmDialogue.Hide();
            }

            confirmDialogue.onResult += OnConfirmDialogueOnonResult;
            spawnPositionSelector.OnSettle += OnSpawnPositionSelectorOnOnSettle;
            spawnPositionSelector.OnStartDrag += OnSpawnPositionSelectorOnOnStartDrag;
            
            while (!finished) await Task.Delay( 10 );

            confirmDialogue.onResult -= OnConfirmDialogueOnonResult;
            spawnPositionSelector.OnSettle -= OnSpawnPositionSelectorOnOnSettle;
            spawnPositionSelector.OnStartDrag -= OnSpawnPositionSelectorOnOnStartDrag;
            
            spawnPositionSelector.gameObject.SetActive( false );
            spawnPositionSelector.DestroyPlaceholders();
            
        }

        public void SpawnDefender(Defender prefab, DefenderSpawnStats spawnStats, Vector3 position) {
            var cost = spawnStats.GetCurrentCost();
            if (CoreGameManager.Current.sessionPack.coins < cost) {
                throw new Exception( "coins is not sufficent" );
            }
            Debug.Log( $"{CoreGameManager.Current.sessionPack.coins} === {cost}" );
            CoreGameManager.Current.sessionPack.coins -= cost;
            CoreGameEvents.Current.onSessionCoinModified?.Invoke();
            var defender = Instantiate( prefab, position, Quaternion.identity, transform );
            spawnStats.IncreaseCost();
            CoreGameEvents.Current.OnDefenderSpawn?.Invoke( defender );
            Debug.Log( $"spawned {defender}" );
        }
    }
}