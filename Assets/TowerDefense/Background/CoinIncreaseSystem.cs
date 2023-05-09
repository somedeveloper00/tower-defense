using System;
using System.Collections;
using System.Threading.Tasks;
using DialogueSystem;
using TowerDefense.Bridges.Ad;
using TowerDefense.Common;
using TowerDefense.Data;
using TowerDefense.Data.Database;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Background {
    [CreateAssetMenu( menuName = "TD/Coin Increase System" )]
    public class CoinIncreaseSystem : ScriptableObject {

        const string adId = "64590a1922a1ba63a8a62b16";
        
        public static CoinIncreaseSystem Current;
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
        [SerializeField] ulong adReward = 100;
        
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

        public bool IsCoinBelowThreshold() => PlayerGlobals.Current.ecoProg.coins < (ulong)tooLowThreshold;

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
                return;
            } else if (r == AdManager.RewardAdResult.CancelByUser) {
                msg.UsePresetForAdCancelledByUser();
                await msg.AwaitClose();
                return;
            } else if (r == AdManager.RewardAdResult.Success) {
                await msg.Close();
                await SecureDateTime.PerformTimeSyncFromInternet();
                _lastAdTime = SecureDateTime.GetSecureUtcDateTime();
                setData();
                PlayerGlobals.Current.ecoProg.coins += adReward;
                PlayerGlobals.Current.SetData( SecureDatabase.Current );
                SecureDatabase.Current.Save();
                Debug.Log( $"recieved ad coin rewards" );
            }
            
        }


        void onCoinChanged(ulong coinsBefore) {
            if (coinsBefore > (ulong)tooLowIncrease && PlayerGlobals.Current.ecoProg.coins < (ulong)tooLowThreshold) {
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
            PlayerGlobals.Current.ecoProg.coins += (ulong)tooLowIncrease;
            PlayerGlobals.Current.SetData( SecureDatabase.Current );
            _lastTooLowIncreaseTime = SecureDateTime.GetSecureUtcDateTime();
            setData();
            SecureDatabase.Current.Save();
            onCoinBelowThresholdTimerEnded?.Invoke();
        }

        void loadData() {
            _lastAdTime = SecureDatabase.Current.TryGetString( "coin-last-watch", out var tick )
                ? new DateTime( long.Parse( tick ) )
                : DateTime.UtcNow;
            _lastTooLowIncreaseTime = SecureDatabase.Current.TryGetString( "low-coin-start", out tick )
                ? new DateTime( long.Parse( tick ) )
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