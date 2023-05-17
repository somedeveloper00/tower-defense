using System;
using System.Collections;
using System.Threading.Tasks;
using DialogueSystem;
using SomeDeveloper;
using TowerDefense.Bridges.Ad;
using TowerDefense.Bridges.Analytics;
using TowerDefense.Common;
using TowerDefense.Data;
using TowerDefense.Data.Database;
using TowerDefense.UI.RewardDialogue;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Background {
    [CreateAssetMenu( menuName = "TD/Coin Increase System" )]
    public class CoinIncreaseHelpSystem : ScriptableObject {

        const string adId = "64590a1922a1ba63a8a62b16";
        
        public static CoinIncreaseHelpSystem Current;
        void OnEnable() {
            Current = this;
            GameInitializer.afterSecureDataLoad += (_) => {
                loadData();
                PlayerGlobals.Current.ecoProg.onCoinsChanged += onCoinChanged;
            };
            GameInitializer.beforeSecureDataSave += (_) => {
                setData();
            };
            
            
        }

        [Title("Ad")]
        [PropertyTooltip("In Hours")]
        [SerializeField] float adDelay = 1;
        [SerializeField] long adReward = 100;
        
        [Title( "Increase if too low" )] 
        [SerializeField] int tooLowThreshold = 20;
        [SerializeField] int tooLowIncrease = 20;
        [PropertyTooltip("In Hours")]
        [SerializeField] float tooLowCoindIncreaseDelay;

        DateTime _lastAdTime;
        DateTime _lastTooLowIncreaseTime;
        Coroutine _tooLowCoinCoroutine;
        
        public event Action onCoinBelowThresholdTimerEnded;

        public TimeSpan GetRemainingTimeForNextCoinIncrease() {
            return _lastTooLowIncreaseTime.AddHours( tooLowCoindIncreaseDelay ) - SecureDateTime.GetSecureUtcDateTime();
        }

        public bool IsCoinBelowThreshold() => PlayerGlobals.Current.ecoProg.Coins < tooLowThreshold;

        public bool IsSomethingWrong() => SecureDateTime.IsLoading;
        
        public bool CanWatchAd() {
            if (IsSomethingWrong()) return false;
            return (SecureDateTime.GetSecureUtcDateTime() - _lastAdTime).TotalHours >= adDelay;
        }

        public TimeSpan GetRemainingTimeForNextAd() {
            if (IsSomethingWrong())
                return new((int)( adDelay / 24 ), (int)( adDelay % 24 ), (int)( 60 * ( adDelay % 1 ) ));
            return _lastAdTime.AddHours( adDelay ) - SecureDateTime.GetSecureUtcDateTime();
        }

        public async Task WatchAd() {
            if (!CanWatchAd()) return;
            
            var msg = DialogueManager.Current.GetOrCreate<MessageDialogue>();
            msg.UsePresetForLoadingAd();
            
            var r = await AdManager.Current.ShowFullScreenRewardVideoAd( adId );
            
            if (r == AdManager.RewardAdResult.Fail) {
                msg.UsePresetForAdFailed();
                await msg.AwaitClose();
            } else if (r == AdManager.RewardAdResult.CancelByUser) {
                msg.UsePresetForAdCancelledByUser();
                await msg.AwaitClose();
            } else if (r == AdManager.RewardAdResult.Success) {
                await msg.Close();
                await SecureDateTime.PerformTimeSyncFromInternet();
                _lastAdTime = SecureDateTime.GetSecureUtcDateTime();
                
                // give reward
                setData();
                var rd = DialogueManager.Current.GetOrCreate<RewardDialogue>();
                rd.showCoinShower = false;
                rd.showSparkles = false;
                rd.coins = adReward;
                rd.setDataAndSave = true;
                rd.detail = "HelpAd";
                await rd.AwaitClose();
                Debug.Log( $"recieved ad coin rewards" );
            }
            
        }


        void onCoinChanged(long coinsBefore) {
            if (coinsBefore > tooLowIncrease && PlayerGlobals.Current.ecoProg.Coins < tooLowThreshold) {
                _lastTooLowIncreaseTime = SecureDateTime.GetSecureUtcDateTime();
                setData();
                SecureDatabase.Current.Save();
                if (_tooLowCoinCoroutine is not null) 
                    BackgroundRunner.Current.StopCoroutine( _tooLowCoinCoroutine );
                _tooLowCoinCoroutine = BackgroundRunner.Current.StartCoroutine( addCoinRoutine() );
            }

            IEnumerator addCoinRoutine() {
                yield return new WaitForSecondsRealtime( tooLowCoindIncreaseDelay * 360 ); // hours to seconds
                addBelowThresholdCoin();
            }
        }

        void addBelowThresholdCoin() {
            Debug.Log( $"coin below threshold timer ended. giving coins..." );
            PlayerGlobals.Current.ecoProg.AddToCoin( GameAnalyticsHelper.ItemType.ItemType_Ad, "HelpAd", tooLowIncrease );
            PlayerGlobals.Current.SetData( SecureDatabase.Current );
            _lastTooLowIncreaseTime = SecureDateTime.GetSecureUtcDateTime();
            setData();
            SecureDatabase.Current.Save();
            onCoinBelowThresholdTimerEnded?.Invoke();
        }

        void loadData() {
            _lastAdTime = SecureDatabase.Current.GetString( "coin-last-watch", out var ticks )
                ? new DateTime( long.Parse( ticks ) )
                : DateTime.UtcNow;
            _lastTooLowIncreaseTime = SecureDatabase.Current.GetString( "low-coin-start", out ticks )
                ? new DateTime( long.Parse( ticks ) )
                : DateTime.UtcNow;

            // check if it's time to give below-threshold-coin reward (user might have left session & came back after 
            // the required time has passes)
            if (IsCoinBelowThreshold()) {
                if (SecureDateTime.GetSecureUtcDateTime() > _lastTooLowIncreaseTime.AddHours( tooLowCoindIncreaseDelay )) {
                    addBelowThresholdCoin();
                }
            }

        }
        
        void setData() {
            SecureDatabase.Current.Set( "coin-last-watch", _lastAdTime.Ticks.ToString() );
            SecureDatabase.Current.Set( "low-coin-start", _lastTooLowIncreaseTime.Ticks.ToString() );
            Debug.Log( "last coin ad time saved" );
        }
    }
}