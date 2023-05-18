using System.Collections;
using AnimFlex.Core.Proxy;
using AnimFlex.Sequencer;
using AnimFlex.Sequencer.BindingSystem;
using AnimFlex.Tweening;
using DialogueSystem;
using RTLTMPro;
using TowerDefense.Background;
using TowerDefense.Bridges.Analytics;
using TowerDefense.Data;
using TowerDefense.Data.Database;
using TriInspector;
using UnityEngine;

namespace TowerDefense.UI {
    public class RewardDialogue : Dialogue {
        
        [Title("Parameters")]
        public long coins;
        public bool showCoinShower = false;
        public bool showSparkles = false;
        public bool setDataAndSave = true;
        public bool waitForUserConfirmation = true;
        public bool useCustomCoinDisplayTarget;
        public CoinDisplay coinDisplayTarget;
        public GameAnalyticsHelper.ItemType itemType = GameAnalyticsHelper.ItemType.ItemType_GameStart;
        public string detail = "reward";

        [Title( "References" )]
        [SerializeField] SequenceAnim inSeq;
        [SerializeField] SequenceAnim outSeq;
        [SerializeField] DelayedButton bckBtn;
        [SerializeField] RTLTextMeshPro rewardCoinAmountTxt;
        [SerializeField] GameObject userConfirmContainer;
        [SerializeField] CoinDisplay defaultCoinDisplayTarget;

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
            
            // for particle system
            if (UiCamera.Current && UiCamera.Current.Camera) {
                canvas.worldCamera = UiCamera.Current.Camera;
                canvas.planeDistance = UiCamera.Current.Camera.nearClipPlane // on top of most things
                                       + 5; // give room for particles 
            }

            StartCoroutine( processRoutine() );
        }

        IEnumerator processRoutine() {

            // init
            rewardCoinAmountTxt.text = coins.ToString( "#,0" ).En2PerNum();
            canvasRaycaster.enabled = false;
            if (!showCoinShower) inSeq.sequence.RemoveClipNode( inSeq.sequence.GetClipNode( coinShowerClipName ) );
            if (!showSparkles) inSeq.sequence.RemoveClipNode( inSeq.sequence.GetClipNode( sparkleClipName ) );
            if (useCustomCoinDisplayTarget) defaultCoinDisplayTarget.gameObject.SetActive( false );
            else coinDisplayTarget = defaultCoinDisplayTarget;
            coinDisplayTarget.fakeOffset = -coins;
            userConfirmContainer.SetActive( waitForUserConfirmation );

            // play in-seq
            inSeq.PlaySequence();
            yield return new WaitForTask( inSeq.AwaitComplete() );
            Debug.Log( $"inseq finished" );

            // wait for confirmation
            if (waitForUserConfirmation) {
                canvasRaycaster.enabled = true;
                bool confirm = false;
                bckBtn.onClick.AddListener( () => confirm = true );
                while (!confirm) yield return new WaitForSecondsRealtime( 0.1f );
            }
            else {
                yield return new WaitForSecondsRealtime( 1 );
            }
            
            // play out-seq
            canvasRaycaster.enabled = false;
            var coinDisplayParent = coinDisplayTarget.transform.parent;
            var coinDisplayCoinIcon = coinDisplayTarget.coinIcon;
            outSeqCoinDisplayTransformBind.value = coinDisplayTarget.transform;
            outSeqCoinDisplayTransformBind.Bind();
            outSeqCoinDisplayGameObjectBind.value = coinDisplayTarget.gameObject;
            outSeqCoinDisplayGameObjectBind.Bind();
            outSeqCoinDisplayParentBind.value = coinDisplayParent;
            outSeqCoinDisplayParentBind.Bind();
            coinSeqCoinIconTransformBind.value = coinDisplayCoinIcon.transform;
            coinSeqCoinIconTransformBind.Bind();
            
            outSeq.PlaySequence();
            outSeq.sequence.onComplete += () => Debug.Log( "outseq completed" );
            
            // play text coin transfer
            yield return new WaitForSecondsRealtime( coinTransferStartTime );
            var c1 = coinDisplayTarget.fakeOffset;
            Tweener.Generate(
                    () => c1,
                    (v) => {
                        c1 = v;
                        coinDisplayTarget.fakeOffset = v;
                        coinDisplayTarget.UpdateView( true );
                    },
                    endValue: 0,
                    proxy: AnimFlexCoreProxyUnscaled.Default )
                .SetDuration( coinTransferDuration )
                .SetEase( coinTransferEase )
                .AddOnComplete( () => Debug.Log( "t1 completed" ) );
            
            var c2 = coins;
            Tweener.Generate(
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
            
            Debug.Log( $"reward anims ended" );

            if (setDataAndSave) {
                // set and save data
                PlayerGlobals.Current.ecoProg.AddToCoin( itemType, detail, coins   );
                PlayerGlobals.Current.SetData( SecureDatabase.Current );
                SecureDatabase.Current.Save();
            }
            
            Close();
        }
    }
}