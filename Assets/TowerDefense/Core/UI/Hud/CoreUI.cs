using System.Collections.Generic;
using System.Linq;
using RTLTMPro;
using TowerDefense.Core.DefenderSpawn;
using TowerDefense.Data;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.UI {
    public class CoreUI : MonoBehaviour {
        public static CoreUI Current;
        [SerializeField] RTLTextMeshPro coinsAmountTxt;
        [SerializeField] List<DefenderSelectionElement> defenderSelectionElements;

#if UNITY_EDITOR
        [Button]
        void getChildSelectionElements() {
            defenderSelectionElements = GetComponentsInChildren<DefenderSelectionElement>().ToList();
        }
#endif

        void OnEnable() {
            Current = this;
            CoreGameEvents.Current.onSessionCoinModified += onCoinModified;
        }

        void OnDisable() {
            CoreGameEvents.Current.onSessionCoinModified -= onCoinModified;
        }

        void Start() {
            var defenders = CoreGameManager.Current.sessionPack.defenders;
            foreach (var element in defenderSelectionElements) {
                var active = defenders.Contains( element.selectionName );
                element.SetActive( active );
                if (active) {
                    element.UpdateInteracableState();
                    element.OnSpawnRequest += onDefenderSpawnClick;
                }
            }
        }

        void onCoinModified() {
            coinsAmountTxt.text = CoreGameManager.Current.sessionPack.coins.ToString("#,0");
        }

        void onDefenderSpawnClick(DefenderSelectionElement element) {
            DefenderSpawner.Current.SpawnUsingSelector( element.selectionName );
        }
    }
}