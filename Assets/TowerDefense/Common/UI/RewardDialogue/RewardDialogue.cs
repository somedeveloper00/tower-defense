using System.Collections;
using AnimFlex.Core.Proxy;
using AnimFlex.Sequencer;
using AnimFlex.Sequencer.BindingSystem;
using AnimFlex.Tweening;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Background;
using TowerDefense.Bridges.Analytics;
using TowerDefense.Core.Hud;
using TowerDefense.Data;
using TowerDefense.Data.Database;
using TowerDefense.Lobby;
using TriInspector;
using UnityEngine;

namespace TowerDefense.UI.RewardDialogue {
    public class RewardDialogue : Dialogue {
        public long coins;
        public bool showCoinShower;
        public bool showSparkles;
        public bool setDataAndSave = true;
        public GameAnalyticsHelper.ItemType itemType = GameAnalyticsHelper.ItemType.ItemType_GameStart;
        public string detail = "reward";
        
        [Title( "References" )] 
        [SerializeField] SequenceAnim inSeq;
        [SerializeField] SequenceAnim outSeq;
        [SerializeField] DelayedButton bckBtn;
        [SerializeField] RTLTextMeshPro rewardCoinAmountTxt;

        [Title("In Seq Anim Params")]
        [SerializeField] string coinShowerClipName;
        [SerializeField] string sparkleClipName;

        [Title( "Out Seq Anim Params" )] 
        [SerializeField] SequenceBinder_Transform outSeqCoinDisplayTransformBind;
        [SerializeField] SequenceBinder_Transform outSeqCoinDisplayParentBind;
        [SerializeField] SequenceBinder_GameObject outSeqCoinDisplayGameObjectBind;
        
        
        [Title("Coin Fx Seq Anim Params")]
        [SerializeField] SequenceBinder_Transform coinSeqCoinIconTransformBind;
        
        [Title("Out Seq Text Anim Params")]
        [SerializeField] float coinTransferStartTime;
        [SerializeField] float coinTransferDuration;
        [SerializeField] Ease coinTransferEase;
        

        protected override void Start() {
            base.Start();

            rewardCoinAmountTxt.text = coins.ToString( "#,0" ).En2PerNum();
            StartCoroutine( processRoutine() );
        }

        IEnumerator processRoutine() {

            // play in-seq
            canvasRaycaster.enabled = false;
            if (!showCoinShower) inSeq.sequence.RemoveClipNode( inSeq.sequence.GetClipNode( coinShowerClipName ) );
            if (!showSparkles) inSeq.sequence.RemoveClipNode( inSeq.sequence.GetClipNode( sparkleClipName ) );
            inSeq.PlaySequence();
            yield return new WaitForTask( inSeq.AwaitComplete() );
            Debug.Log( $"inseq finished" );

            // wait for confirmation
            canvasRaycaster.enabled = true;
            bool confirm = false;
            bckBtn.onClick.AddListener( () => confirm = true );
            while (!confirm) yield return new WaitForSecondsRealtime( 0.1f );
            
            // play out-seq
            canvasRaycaster.enabled = false;
            var coinDisplay = findCoinDisplay();
            var coinDisplayParent = coinDisplay.transform.parent;
            var coinDisplayCoinIcon = coinDisplay.coinIcon;
            outSeqCoinDisplayTransformBind.value = coinDisplay.transform;
            outSeqCoinDisplayTransformBind.Bind();
            outSeqCoinDisplayGameObjectBind.value = coinDisplay.gameObject;
            outSeqCoinDisplayGameObjectBind.Bind();
            outSeqCoinDisplayParentBind.value = coinDisplayParent;
            outSeqCoinDisplayParentBind.Bind();
            coinSeqCoinIconTransformBind.value = coinDisplayCoinIcon.transform;
            coinSeqCoinIconTransformBind.Bind();
            
            outSeq.PlaySequence();
            outSeq.sequence.onComplete += () => Debug.Log( "outseq completed" );
            
            // play text coin transfer
            yield return new WaitForSecondsRealtime( coinTransferStartTime );
            var c1 = PlayerGlobals.Current.ecoProg.Coins;
            var t1 = Tweener.Generate(
                    () => c1,
                    (v) => {
                        c1 = v;
                        coinDisplay.coinTxt.text = c1.ToString( "#,0" ).En2PerNum();
                    },
                    endValue: PlayerGlobals.Current.ecoProg.Coins + coins,
                    proxy: AnimFlexCoreProxyUnscaled.Default )
                .SetDuration( coinTransferDuration )
                .SetEase( coinTransferEase )
                .AddOnComplete( () => Debug.Log( "t1 completed" ) );
            
            var c2 = coins;
            var t2 = Tweener.Generate(
                    () => c2,
                    (v) => {
                        c2 = v;
                        rewardCoinAmountTxt.text = c2.ToString( "#,0" ).En2PerNum();
                    }, 
                    endValue: 0, 
                    proxy: AnimFlexCoreProxyUnscaled.Default )
                .SetDuration( coinTransferDuration )
                .SetEase( coinTransferEase )
                .AddOnComplete( () => Debug.Log( "t2 completed" ) );

            yield return new WaitForTask( outSeq.AwaitComplete() );
            // yield return new WaitForTask( t1.AwaitComplete() );
            // yield return new WaitForTask( t2.AwaitComplete() );
            
            Debug.Log( $"reward anims ended" );

            if (setDataAndSave) {
                // set and save data
                PlayerGlobals.Current.ecoProg.AddToCoin( itemType, detail, PlayerGlobals.Current.ecoProg.Coins  );
                PlayerGlobals.Current.SetData( SecureDatabase.Current );
                SecureDatabase.Current.Save();
            }
            
            Close();
        }

        CoinDisplay findCoinDisplay() {
            if (LobbyManager.Current) {
                if (LobbyManager.Current.coinDisplay) return LobbyManager.Current.coinDisplay;
                var cd = LobbyManager.Current.GetComponentInChildren<CoinDisplay>();
                if (cd) return cd;
            }

            if (CoreHud.Current) {
                if (CoreHud.Current.coinDisplay) return CoreHud.Current.coinDisplay;
                var cd = CoreHud.Current.GetComponentInChildren<CoinDisplay>();
                if (cd) return cd;
            }

            return FindObjectOfType<CoinDisplay>();
        }
    }
}