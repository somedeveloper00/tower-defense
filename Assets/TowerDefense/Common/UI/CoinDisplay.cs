using System.Collections;
using RTLTMPro;
using TowerDefense.Background;
using TowerDefense.Data;
using TriInspector;
using UnityEngine;

namespace TowerDefense.UI {
    public class CoinDisplay : MonoBehaviour {
        [SerializeField] RTLTextMeshPro coinTxt;
        [SerializeField] RTLTextMeshPro adRemainingTimeTxt;
        [SerializeField] DelayedButton adBtn;
        [SerializeField] GameObject adReadyContainer;
        [SerializeField] GameObject adNotReadyContainer;
        [SerializeField] GameObject adUpdatingContainer;
        [PropertyTooltip("{0}: Hours, {1}: Minutes, {2}: Seconds")]
        [SerializeField] string adRemainingTimeFormat;
        
        void Start() {
            adBtn.onClick.AddListener( onAdClick );
            StartCoroutine( routine() );
            
            IEnumerator routine() {
                do {
                    updateView();
                    yield return new WaitForSecondsRealtime( 0.25f );
                } while (true);
            }
        }
        

        void updateView() {
            if (coinTxt)
                coinTxt.text = PlayerGlobals.Current.ecoProg.coins.ToString( "#,0" ).En2PerNum();
            
            // update ad stuff
            var canWatch = CoinRewardAdSystem.Current.CanWatchAd();
            if (adReadyContainer)
                adReadyContainer.SetActive( canWatch );
            if (adNotReadyContainer)
                adNotReadyContainer.SetActive( !canWatch );
            if (adRemainingTimeTxt) {
                var t = CoinRewardAdSystem.Current.GetRemainingTimeForNextAd();
                adRemainingTimeTxt.text = adRemainingTimeFormat
                    .Replace( "{0}", t.Hours.ToString() )
                    .Replace( "{1}", t.Minutes.ToString("00") )
                    .Replace( "{2}", t.Seconds.ToString("00") );
            }

            if (adUpdatingContainer) {
                adUpdatingContainer.SetActive( CoinRewardAdSystem.Current.IsSomethingWrong() );
            }
        }

        void onAdClick() {
        #pragma warning disable CS4014
            CoinRewardAdSystem.Current.WatchAd();
        #pragma warning restore CS4014
        }
    }
}