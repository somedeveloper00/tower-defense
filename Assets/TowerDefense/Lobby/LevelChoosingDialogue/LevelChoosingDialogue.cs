﻿using System.Collections.Generic;
using AnimFlex.Sequencer;
using DialogueSystem;
using TowerDefense.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    public class LevelChoosingDialogue : Dialogue {
        [SerializeField] LevelStartButton levelStartButtonInstance;
        [SerializeField] Button backgroundBtn;
        [SerializeField] Button backBtn;
        [SerializeField] SequenceAnim inSeq, outSeq;

        List<LevelStartButton> _levelViews = new();

        protected override async void Start() {
            base.Start();
            setupLevelViews();
            backBtn.onClick.AddListener( CloseWithAnim );
            backgroundBtn.onClick.AddListener( CloseWithAnim );
            
            canvasRaycaster.enabled = false;
            Debug.Log( $"{Time.frameCount} reqed" );
            inSeq.PlaySequence();
            await inSeq.AwaitComplete();
            canvasRaycaster.enabled = true;
        }

        void CloseWithAnim() {
            canvasRaycaster.enabled = false;
            outSeq.PlaySequence();
            outSeq.sequence.onComplete += Close;
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