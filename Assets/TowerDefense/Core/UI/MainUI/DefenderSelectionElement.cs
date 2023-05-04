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
            
        [SerializeField] RTLTextMeshPro coinTxt, titleTxt;
        [SerializeField] CanvasGroup container;
        [SerializeField] Button btn;
        [SerializeField] Image icon;

        DefenderSpawnStats _spawnStats;

        public void Start() {
            var defender = DefenderDatabase.Current.GetDefenderMainPrefab( selectionName );
            _spawnStats = defender.spawnStats;
            titleTxt.text = defender.name;
            icon.sprite = Sprite.Create( defender.icon, new Rect(0, 0, defender.icon.width, defender.icon.height), Vector2.zero );
            btn.onClick.AddListener( () => OnSpawnRequest?.Invoke( this ) );
            CoreGameEvents.Current.onSessionCoinModified += UpdateCostAndInteractable;
            CoreGameEvents.Current.OnDefenderSpawn += onDefenderSpawn;
        }

        void OnDestroy() {
            CoreGameEvents.Current.onSessionCoinModified -= UpdateCostAndInteractable;
            CoreGameEvents.Current.OnDefenderSpawn -= onDefenderSpawn;
        }

        void onDefenderSpawn(Defender _) => UpdateCostAndInteractable(); 
        
        public void UpdateCostAndInteractable() {
            coinTxt.text = _spawnStats.GetCurrentCost().ToString("#,0");
            container.interactable = _spawnStats.GetCurrentCost() <= CoreGameManager.Current.sessionPack.coins;
        }

        /// <summary>
        /// set active state
        /// </summary>
        public void SetActive(bool active) {
            gameObject.SetActive( active );
        }
    }
}