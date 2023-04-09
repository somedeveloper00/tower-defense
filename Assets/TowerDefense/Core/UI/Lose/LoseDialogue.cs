using System;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TowerDefense.Ad;
using TowerDefense.Common;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI.Lose {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class LoseDialogue : Dialogue {

        [SerializeField] string adSorryTitleTxt;
        [SerializeField] string adSorryTxt;
        [GroupNext("ref")]
        [SerializeField] SequenceAnim inSequence, outSequence;
        [SerializeField] Button retryBtn, returnBtn;
        

        protected override void OnEnable() {
            base.OnEnable();
            
            canvasRaycaster.enabled = false;
            canvasGroup.alpha = 0;
            
            retryBtn.onClick.AddListener( onRetryBtnClick );
            returnBtn.onClick.AddListener( onReturnBtnClick );
        }

        protected override void Start() {
            base.Start();
            inSequence.PlaySequence();
            inSequence.sequence.onComplete -= enableListeners;
            inSequence.sequence.onComplete += enableListeners;
            
            void enableListeners() => canvasRaycaster.enabled = true;
        }
        
        
        async void onRetryBtnClick() {
            // show rewarded ad
            canvasRaycaster.enabled = false;
            var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>( transform.parent );
            loading.UsePresetForLoadingAd();
            var result = await AdManager.Current.ShowFullScreenRewardVideoAd( "643006352eeae447e5ae5bd3" );
            await loading.Close();
            
            if (result == AdManager.RewardAdResult.Success) {
                CoreGameManager.Current.RestartGame();
            }
            else if (result == AdManager.RewardAdResult.CancelByUser) {
                var msgDialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>( transform.parent );
                msgDialogue.UsePresetForAdCancelledByUser();
                await msgDialogue.AwaitClose();
                canvasRaycaster.enabled = true;
            }else if (result == AdManager.RewardAdResult.Fail) {
                var msgDialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>( transform.parent );
                msgDialogue.UsePresetForAdCancelledByUser();
                await msgDialogue.AwaitClose();
                canvasRaycaster.enabled = true;
            }
        }
        
        void onReturnBtnClick() {
            canvasRaycaster.enabled = false;
            CoreGameManager.Current.BackToLobby();
            outSequence.PlaySequence();
        }
    }
}