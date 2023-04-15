using System;
using RTLTMPro;
using TowerDefense.Core.Defenders;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI {
    public class DefenderSelectionElement : MonoBehaviour {
#if UNITY_EDITOR
        [Dropdown(nameof(selectionOptions))]
#endif
        public string selectionName;

#if UNITY_EDITOR
        string[] selectionOptions => DefenderDatabase.Current?.GetAllNames() ?? new string[0];
        
#endif

        public event Action<DefenderSelectionElement> OnSpawnRequest; 
            
        [SerializeField] RTLTextMeshPro coinTxt;
        [SerializeField] CanvasGroup container;
        [SerializeField] Button btn;

        ulong cost;

        public void Start() {
            var defender = DefenderDatabase.Current.GetDefenderMainPrefab( selectionName );
            cost = defender.cost;
            coinTxt.text = defender.cost.ToString("#,0");
            btn.onClick.AddListener( () => OnSpawnRequest?.Invoke( this ) );
            CoreGameEvents.Current.onSessionCoinModified += UpdateInteracableState;
        }

        void OnDestroy() {
            CoreGameEvents.Current.onSessionCoinModified -= UpdateInteracableState;
        }

        /// <summary>
        ///  update clickable state
        /// </summary>
        public void UpdateInteracableState() {
            container.interactable = cost <= CoreGameManager.Current.sessionPack.coins;
        }

        /// <summary>
        /// set active state
        /// </summary>
        public void SetActive(bool active) {
            gameObject.SetActive( active );
        }
    }
}