using System.Collections.Generic;
using System.Linq;
using RTLTMPro;
using TowerDefense.Core.DefenderSpawn;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI {
    public class CoreUI : MonoBehaviour {
        public static CoreUI Current;
        [SerializeField] RTLTextMeshPro coinsAmountTxt;
        [SerializeField] List<DefenderSelectionElement> defenderSelectionElements;
        [SerializeField] CanvasGroup selectionCanvasGroup;
        [SerializeField] RTLTextMeshPro heartTxt;
        [SerializeField] RTLTextMeshPro timerTxt;

#if UNITY_EDITOR
        [Button]
        void getChildSelectionElements() {
            defenderSelectionElements = GetComponentsInChildren<DefenderSelectionElement>().ToList();
        }
#endif

        void OnEnable() {
            Current = this;
            CoreGameEvents.Current.onSessionCoinModified += onCoinModified;
            CoreGameEvents.Current.onLifeModified += onLifeModified;
            CoreGameEvents.Current.onTimeModified += onTimeModified;
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
                    element.OnSpawnRequest += onDefenderSpawnClick;
                }
            }
        }

        void onTimeModified() {
            timerTxt.text = ( CoreGameManager.Current.gameTime / 60 ).ToString( "00" ).En2PerNum() + ":" +
                            ( CoreGameManager.Current.gameTime % 60 ).ToString( "00" ).En2PerNum();
        }
        void onLifeModified() {
            heartTxt.text = CoreGameManager.Current.life.ToString().En2PerNum();
        }

        void onCoinModified() {
            coinsAmountTxt.text = CoreGameManager.Current.sessionPack.coins.ToString("#,0").En2PerNum();
        }

        async void onDefenderSpawnClick(DefenderSelectionElement element) {
            Debug.Log( $"spawn request for {element.selectionName}" );
            selectionCanvasGroup.interactable = false;
            await DefenderSpawner.Current.SpawnUsingSelector( element.selectionName );
            selectionCanvasGroup.interactable = true;
        }
    }
}