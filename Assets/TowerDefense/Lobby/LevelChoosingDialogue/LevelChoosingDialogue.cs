using System.Collections.Generic;
using DialogueSystem;
using TowerDefense.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.LevelChoosing {
    public class LevelChoosingDialogue : Dialogue {
        [SerializeField] LevelStartButton levelStartButtonInstance;
        [SerializeField] Button backBtn;

        List<LevelStartButton> _levelViews = new();

        protected override void Start() {
            base.Start();
            setupLevelViews();
            backBtn.onClick.AddListener( Close );
        }

        void setupLevelViews() {
            foreach (var levelView in _levelViews)
                Destroy( levelView.gameObject );
            var levelDatas = PlayerGlobals.Current.levelsData.coreLevels;
            for (var i = 0; i < levelDatas.Count; i++) {
                var lvlView = Instantiate( levelStartButtonInstance, levelStartButtonInstance.transform.parent );
                lvlView.gameObject.SetActive( true );
                lvlView.coreLevelData = levelDatas[i];
                lvlView.levelProgress = PlayerGlobals.Current.GetOrCreateLevelProg( levelDatas[i].id );
                _levelViews.Add( lvlView );
            }
        }
    }
}