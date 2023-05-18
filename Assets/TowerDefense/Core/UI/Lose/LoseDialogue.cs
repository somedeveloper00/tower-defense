using System.Threading.Tasks;
using AnimFlex.Sequencer;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Bridges.Ad;
using TowerDefense.UI;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.Core.UI.Lose {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class LoseDialogue : Dialogue {
        
        const string RETRY_AD_Id = "643006352eeae447e5ae5bd3";

        public LoseData loseData;

        [GroupNext( "ref" )] 
        [SerializeField] SequenceAnim inSeq;
        [SerializeField] RTLTextMeshPro coinAmountTxt;
        [SerializeField] Button retryBtn, returnBtn;
        [SerializeField] CoinDisplay coinDisplay;
        

        protected override async void Start() {
            base.Start();
            canvasRaycaster.enabled = false;
            
            retryBtn.onClick.AddListener( onRetryBtnClick );
            returnBtn.onClick.AddListener( onReturnBtnClick );
            coinAmountTxt.text = loseData.coins.ToString( "#,0" );
            coinDisplay.fakeOffset = -loseData.coins;
            
            inSeq.PlaySequence();
            await inSeq.AwaitComplete();

            if (loseData.coins > 0) {
                var d = DialogueManager.Current.GetOrCreate<RewardDialogue>( parent: this );
                d.useCustomCoinDisplayTarget = true;
                d.coinDisplayTarget = coinDisplay;
                d.setDataAndSave = false;
                d.showCoinShower = false;
                d.showSparkles = false;
                d.coins = loseData.coins;
                d.waitForUserConfirmation = false;
                await d.AwaitClose();
            }
            
            canvasRaycaster.enabled = true;
        }
        
        
        async void onRetryBtnClick() {
            // show rewarded ad
            canvasRaycaster.enabled = false;
            var loading = DialogueManager.Current.GetOrCreate<MessageDialogue>( transform.parent );
            loading.UsePresetForLoadingAd();
            var result = await AdManager.Current.ShowFullScreenRewardVideoAd( RETRY_AD_Id );
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
        
        async void onReturnBtnClick() {
            await Task.Delay( 1000 );
            canvasRaycaster.enabled = false;
            CoreGameManager.Current.BackToLobby();
        }
    }
}