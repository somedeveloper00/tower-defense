using System;
using System.Collections.Generic;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Common;
using TowerDefense.Data;
using TowerDefense.Data.Progress;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    public class LevelStartButton : MonoBehaviour {
        [SerializeField] Button playBtn;
        [SerializeField] RTLTextMeshPro title;
        [SerializeField] List<long> possibleSessionCoins = new();
        [SerializeField] List<string> possibleSessionCoinsTxt = new();
        [SerializeField] string coinChooseMessageTitleText;
        [SerializeField, TextArea] string coinChooseMessageBodyText;
        [SerializeField] MessageDialogue.IconType coinChooseMessageIcon;
        [SerializeField] string notEnoughCoinsTitleText;
        [SerializeField, TextArea] string notEnoughCoinsBodyText;
        [SerializeField] MessageDialogue.IconType notEnoughCoinsIcon;

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
            int count = 0;
            for (int i = 0; i < possibleSessionCoins.Count; i++) {
                if (PlayerGlobals.Current.ecoProg.Coins >= possibleSessionCoins[i]) {
                    dialogue.AddButton( possibleSessionCoinsTxt[i], "ok" );
                    count++;
                }
            }

            if (count == 0) {
                dialogue.SetTitleText( notEnoughCoinsTitleText );
                dialogue.SetBodyText( notEnoughCoinsBodyText );
                dialogue.SetIcon( notEnoughCoinsIcon );
                dialogue.AddOkButton();
                await dialogue.AwaitClose();
                return;
            }
            dialogue.SetTitleText( coinChooseMessageTitleText );
            dialogue.SetBodyText( coinChooseMessageBodyText );
            dialogue.SetIcon( coinChooseMessageIcon );
            await dialogue.AwaitClose();


            var ind = possibleSessionCoinsTxt.IndexOf( dialogue.result );
            if (ind != -1) {
                var coins = possibleSessionCoins[ind];
                LobbyManager.Current.StartGame( coreLevelData, new() {
                    coins = coins,
                    defenders = new List<string>() { "monkey", "shooter" }
                } );
            }
        }
    }
}