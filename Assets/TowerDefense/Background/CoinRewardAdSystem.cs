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
    [CreateAssetMenu( menuName = "TD/CoinRewardAdSystem" )]
    public class CoinRewardAdSystem : ScriptableObject {

        public static CoinRewardAdSystem Current;
        void OnEnable() {
            Current = this;
            GameInitializer.onInitTasks.Add( new GameInitializer.OnInitTask( 10, () => {
                load();
                return Task.CompletedTask;
            } ) );
        }

        [PropertyTooltip("In Hours")]
        [SerializeField] float adDelay = 1;
        [SerializeField] ulong adReward = 100;

        const string adId = "64590a1922a1ba63a8a62b16";

        DateTime _lastAdTime;

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
                save();
                PlayerGlobals.Current.ecoProg.coins += adReward;
                PlayerGlobals.Current.SetData( SecureDatabase.Current );
                SecureDatabase.Current.Save();
                Debug.Log( $"recieved ad coin rewards" );
            }
            
        }


        void load() {
            if (SecureDatabase.Current.TryGetString( "coin-last-watch", out var tick )) {
                _lastAdTime = new DateTime( long.Parse( tick ) );
                Debug.Log( "last coin ad time loaded" );
            }
            else {
                _lastAdTime = DateTime.UtcNow;
                Debug.Log( "last coin ad time could not load" );
            }
        }
        
        void save() {
            SecureDatabase.Current.Set( "coin-last-watch", _lastAdTime.Ticks.ToString() );
            Debug.Log( "last coin ad time saved" );
        }
    }
}