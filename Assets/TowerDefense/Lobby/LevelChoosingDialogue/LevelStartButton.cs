using System;
using System.Collections.Generic;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Common;
using TowerDefense.Data;
using TowerDefense.Data.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby.LevelChoosing {
    public class LevelStartButton : MonoBehaviour {
        [SerializeField] Button playBtn;
        [SerializeField] RTLTextMeshPro title;
        [SerializeField] List<ulong> possibleSessionCoins = new();
        [SerializeField] List<string> possibleSessionCoinsTxt = new();
        [SerializeField, TextArea] string coinChooseMessageTitleText;
        [SerializeField, TextArea] string coinChooseMessageBodyText;
        [SerializeField] MessageDialogue.IconType coinChooseMessageIcon;

        [NonSerialized] public CoreLevelData coreLevelData;
        [NonSerialized] public LevelProgress.Level levelProgress;

        void Start() {
            playBtn.onClick.AddListener( onPlayButtonClick );
            playBtn.interactable = levelProgress.status.HasFlag( LevelProgress.LevelStatus.Unlocked );
            title.text = coreLevelData.title;
        }

        async void onPlayButtonClick() {
            var dialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>();

            // get possible coin packs
            for (int i = 0; i < possibleSessionCoins.Count; i++) {
                if (PlayerGlobals.Current.ecoProg.coins >= possibleSessionCoins[i]) {
                    dialogue.AddButton( possibleSessionCoinsTxt[i], "ok" );
                }
            }

            dialogue.SetTitleText( coinChooseMessageTitleText );
            dialogue.SetBodyText( coinChooseMessageBodyText );
            dialogue.SetIcon( coinChooseMessageIcon );
            await dialogue.AwaitClose();

            var ind = possibleSessionCoinsTxt.IndexOf( dialogue.result );
            if (ind != -1) {
                ulong coins = possibleSessionCoins[ind];
                LobbyManager.Current.StartGame( coreLevelData, new() {
                    coins = coins,
                    defenders = new List<string>() { "monkey", "shooter" }
                } );
            }
        }
    }
}