using System.Collections.Generic;
using DialogueSystem;
using TowerDefense.Player;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.LevelChoosing {
    public class LevelChoosingDialogue : Dialogue {
        [SerializeField] LevelStartButton levelStartButtonInstance;
        [SerializeField] Button backBtn;

        List<LevelStartButton> _levelViews = new();
        List<GameLevelsData.Level> _levels = new();

        protected override void Start() {
            base.Start();
            _levels = PlayerGlobals.Current.gameLevelsData.levels;
            setupLevelViews();
            backBtn.onClick.AddListener( Close );
        }

        void setupLevelViews() {
            foreach (var levelView in _levelViews)
                Destroy( levelView.gameObject );
            for (var i = 0; i < _levels.Count; i++) {
                var lvlView = Instantiate( levelStartButtonInstance, levelStartButtonInstance.transform.parent );
                lvlView.gameObject.SetActive( true );
                lvlView.levelData = _levels[i];
                _levelViews.Add( lvlView );
            }
        }
    }
}