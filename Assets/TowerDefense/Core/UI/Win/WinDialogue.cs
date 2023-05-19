using System.Threading.Tasks;
using AnimFlex.Sequencer;
using DialogueSystem;
using TowerDefense.UI;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.UI.Win {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class WinDialogue : Dialogue {
        public WinData winData;

        [GroupNext( "ref" )] 
        [SerializeField] SequenceAnim inSeq;
        [SerializeField] GameObject[] starObjects;
        [SerializeField] DelayedButton returnButton;
        [SerializeField] CoinDisplay coinDisplay;
        
        protected override async void Start() {
            base.Start();
            returnButton.onClick.AddListener( onLobbyBtnClick );
            for (int i = 0; i < starObjects.Length; i++) 
                starObjects[i].SetActive( i < winData.stars );
            coinDisplay.fakeOffset = -winData.coins;
            
            canvasRaycaster.enabled = false;
            inSeq.PlaySequence();
            await inSeq.AwaitComplete();
            
            await Task.Delay( 200 );
            
            // wait till focus is on this dialogue
            while(DialogueManager.Current.focusManager.IsDialogueFocused( this )) await Task.Delay( 1000 );
            
            canvasRaycaster.enabled = true;
        }

        async void onLobbyBtnClick() {
            canvasRaycaster.enabled = false;

            var d = DialogueManager.Current.GetOrCreate<RewardDialogue>( parent: this );
            d.coins = winData.coins;
            d.setDataAndSave = false;
            d.showSparkles = true;
            d.showCoinShower = false;
            d.waitForUserConfirmation = false;
            d.useCustomCoinDisplayTarget = true;
            d.coinDisplayTarget = coinDisplay;
            await d.AwaitClose();

            await Task.Delay( 500 );

            CoreGameManager.Current.BackToLobby();
            Close();
        }
    }
}