using System;
using System.Collections;
using TowerDefense.Core.Defenders;
using UnityEngine;

namespace TowerDefense.Core.DefenderSpawn {
    public class DefenderSpawner : MonoBehaviour {

        public SpawnPositionSelector spawnPositionSelector;

        public static DefenderSpawner Current;

        void OnEnable() => Current = this;

        public void SpawnUsingSelector(string defenderName) {
            StartCoroutine( enumerator() );
            Debug.Log( "jooj" );

            IEnumerator enumerator() {
                var placeholder = DefenderDatabase.Current.GetDefenderPlaceholderPrefab( defenderName );
                spawnPositionSelector.gameObject.SetActive( true );
                spawnPositionSelector.SetPlaceholder( placeholder );
                bool selected = false;
                Vector3 result = new();
                spawnPositionSelector.OnSelect += (p) => {
                    selected = true;
                    result = p;
                    spawnPositionSelector.gameObject.SetActive( false );
                };
                yield return new WaitUntil( () => selected );
                SpawnDefender( DefenderDatabase.Current.GetDefenderMainPrefab( defenderName ), result );
            }
        }

        public void SpawnDefender(Defender prefab, Vector3 position) {
            if (CoreGameManager.Current.sessionPack.coins < prefab.cost) {
                throw new Exception( "coins is not sufficent" );
            }
            Debug.Log( $"{CoreGameManager.Current.sessionPack.coins} === {prefab.cost}" );
            CoreGameManager.Current.sessionPack.coins -= prefab.cost;
            CoreGameEvents.Current.onSessionCoinModified?.Invoke();
            var defender = Instantiate( prefab, position, Quaternion.identity, transform );
            CoreGameEvents.Current.OnDefenderSpawn?.Invoke( defender );
            Debug.Log( $"spawned {defender}" );
        }
    }
}