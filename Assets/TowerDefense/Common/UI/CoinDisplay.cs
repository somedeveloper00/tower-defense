using System.Collections;
using AnimFlex.Sequencer.UserEnd;
using RTLTMPro;
using TowerDefense.Background;
using TowerDefense.Data;
using TriInspector;
using UnityEngine;

namespace TowerDefense.UI {
    public class CoinDisplay : MonoBehaviour {
        [SerializeField] RTLTextMeshPro coinTxt;
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
            CoinIncreaseSystem.Current.onCoinBelowThresholdTimerEnded += onCoinBelowThresholdTimerEnded;
            StartCoroutine( routine() );
            
            IEnumerator routine() {
                do {
                    updateView();
                    yield return new WaitForSecondsRealtime( 0.25f );
                } while (true);
            }
        }

        void OnDestroy() {
            CoinIncreaseSystem.Current.onCoinBelowThresholdTimerEnded -= onCoinBelowThresholdTimerEnded;
        }

        async void onCoinBelowThresholdTimerEnded() {
            if (onCoinBelowThresholdTimerEndedSeq) {
                onCoinBelowThresholdTimerEndedSeq.PlaySequence();
                await onCoinBelowThresholdTimerEndedSeq.AwaitComplete();
            }
            updateView();
        }

        void updateView() {
            if (coinTxt)
                coinTxt.text = PlayerGlobals.Current.ecoProg.coins.ToString( "#,0" ).En2PerNum();
            
            // update ad stuff
            var canWatch = CoinIncreaseSystem.Current.CanWatchAd();
            if (adReadyContainer)
                adReadyContainer.SetActive( canWatch );
            if (adNotReadyContainer)
                adNotReadyContainer.SetActive( !canWatch );
            if (adRemainingTimeTxt) {
                var t = CoinIncreaseSystem.Current.GetRemainingTimeForNextAd();
                adRemainingTimeTxt.text = adRemainingTimeFormat
                    .Replace( "{0}", t.Hours.ToString() )
                    .Replace( "{1}", t.Minutes.ToString("00") )
                    .Replace( "{2}", t.Seconds.ToString("00") );
            }

            var isCoinBelowThreshold = CoinIncreaseSystem.Current.IsCoinBelowThreshold();
            
            if (coinBelowThresholdContainer) {
                coinBelowThresholdContainer.SetActive( isCoinBelowThreshold );
            }
            
            if (coinBelowThresholdRemainingTimeTxt) {
                if (isCoinBelowThreshold) {
                    var t = CoinIncreaseSystem.Current.GetRemainingTimeForNextCoinIncrease();
                    coinBelowThresholdRemainingTimeTxt.text = coinBelowThresholdRemainingTimeFormat
                        .Replace( "{0}", t.Minutes.ToString("00") )
                        .Replace( "{1}", t.Seconds.ToString("00") );
                }
            }

            if (adUpdatingContainer) {
                adUpdatingContainer.SetActive( CoinIncreaseSystem.Current.IsSomethingWrong() );
            }
        }

        void onAdClick() {
        #pragma warning disable CS4014
            CoinIncreaseSystem.Current.WatchAd();
        #pragma warning restore CS4014
        }
    }
}