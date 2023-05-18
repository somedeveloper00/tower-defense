using System.Collections;
using AnimFlex.Sequencer;
using RTLTMPro;
using TowerDefense.Background;
using TowerDefense.Data;
using TriInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI {
    public class CoinDisplay : MonoBehaviour {

        public long fakeOffset = 0;
        public Image coinIcon;
        public RTLTextMeshPro coinTxt;
        
        [SerializeField] RTLTextMeshPro adRemainingTimeTxt;
        [SerializeField] RTLTextMeshPro coinBelowThresholdRemainingTimeTxt;
        [SerializeField] DelayedButton adBtn;
        [SerializeField] GameObject adReadyContainer;
        [SerializeField] GameObject adNotReadyContainer;
        [SerializeField] GameObject adUpdatingContainer;
        [SerializeField] GameObject coinBelowThresholdContainer;
        [SerializeField] SequenceAnim onCoinBelowThresholdTimerEndedSeq;
        [PropertyTooltip("{0}: Hours, {1}: Minutes, {2}: Seconds")]
        [SerializeField] string adRemainingTimeFormat;
        [PropertyTooltip("{0}: Minutes, {1}: Seconds")]
        [SerializeField] string coinBelowThresholdRemainingTimeFormat;
        
        void Start() {
            adBtn.onClick.AddListener( onAdClick );
            CoinIncreaseHelpSystem.Current.onCoinBelowThresholdTimerEnded += onCoinBelowThresholdTimerEnded;
            StartCoroutine( routine() );
            
            IEnumerator routine() {
                do {
                    UpdateView();
                    yield return new WaitForSecondsRealtime( 0.25f );
                } while (true);
            }
        }

        void OnDestroy() {
            CoinIncreaseHelpSystem.Current.onCoinBelowThresholdTimerEnded -= onCoinBelowThresholdTimerEnded;
        }

        async void onCoinBelowThresholdTimerEnded() {
            if (onCoinBelowThresholdTimerEndedSeq) {
                onCoinBelowThresholdTimerEndedSeq.PlaySequence();
                await onCoinBelowThresholdTimerEndedSeq.AwaitComplete();
            }
            UpdateView();
        }

        public void UpdateView(bool fast = false) {
            if (coinTxt)
                coinTxt.text = (fakeOffset + PlayerGlobals.Current.ecoProg.Coins).ToString( "#,0" ).En2PerNum();

            if (fast) return;
            var isCoinBelowThreshold = CoinIncreaseHelpSystem.Current.IsCoinBelowThreshold();
            
            if (coinBelowThresholdContainer) {
                coinBelowThresholdContainer.SetActive( isCoinBelowThreshold );
            }
            
            // update ad stuff
            var canWatch = CoinIncreaseHelpSystem.Current.CanWatchAd();
            if (adReadyContainer)
                adReadyContainer.SetActive( canWatch );
            if (adNotReadyContainer)
                adNotReadyContainer.SetActive( !canWatch );
            if (adRemainingTimeTxt) {
                var t = CoinIncreaseHelpSystem.Current.GetRemainingTimeForNextAd();
                adRemainingTimeTxt.text = string.Format( 
                    adRemainingTimeFormat, 
                    t.Hours, t.Minutes.ToString( "00" ), t.Seconds.ToString( "00" ) );
            }

            
            if (coinBelowThresholdRemainingTimeTxt) {
                if (isCoinBelowThreshold) {
                    var t = CoinIncreaseHelpSystem.Current.GetRemainingTimeForNextCoinIncrease();
                    coinBelowThresholdRemainingTimeTxt.text = string.Format( 
                        coinBelowThresholdRemainingTimeFormat,
                        t.Minutes.ToString( "00" ), t.Seconds.ToString( "00" ) );
                }
            }

            if (adUpdatingContainer) {
                adUpdatingContainer.SetActive( CoinIncreaseHelpSystem.Current.IsSomethingWrong() );
            }
        }

        void onAdClick() {
        #pragma warning disable CS4014
            CoinIncreaseHelpSystem.Current.WatchAd( this );
        #pragma warning restore CS4014
        }
    }
}