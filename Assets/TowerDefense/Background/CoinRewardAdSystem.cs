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
                BackgroundRunner.Current.StartCoroutine( syncTimingThroughoutTheGameRoutine() );
                return Task.CompletedTask;
            } ) );
        #pragma warning disable CS4014
            timeJumpCheck();
        #pragma warning restore CS4014
        }

        [PropertyTooltip("In Hours")]
        [SerializeField] float adDelay = 1;
        [SerializeField] ulong adReward = 100;

        const string adId = "64590a1922a1ba63a8a62b16";

        
        DateTime _lastAdTime;
        float _timeDifference = 0;
        
        public bool IsLoading { get; private set; }

        public TimeSpan GetRemainingTimeForNextAd() {
            if (IsLoading)
                return new((int)( adDelay / 24 ), (int)( adDelay % 24 ), (int)( 60 * ( adDelay % 1 ) ));
            return _lastAdTime.AddHours( adDelay ) - DateTime.UtcNow.AddSeconds( _timeDifference );
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
                _lastAdTime = DateTime.UtcNow.AddSeconds( _timeDifference );
                save();
                PlayerGlobals.Current.ecoProg.coins += adReward;
                PlayerGlobals.Current.SetData( SecureDatabase.Current );
                SecureDatabase.Current.Save();
                Debug.Log( $"recieved ad coin rewards" );
            }
            
        }

        public bool CanWatchAd() {
            if (IsLoading) return false;
            return (DateTime.UtcNow.AddSeconds( _timeDifference ) - _lastAdTime).TotalHours >= adDelay;
        }
        

        IEnumerator syncTimingThroughoutTheGameRoutine() {
            do {
                yield return new WaitForSecondsRealtime( 3 );
                yield return new WaitForTask( updateFromInternet() );
            } while (true);
        }

        async Task timeJumpCheck() {
            do {
                DateTime d1 = new(DateTime.UtcNow.Ticks); // get a copy of utc now
                await Task.Delay( 100 );
                if (( DateTime.UtcNow - d1 ).TotalSeconds > 0.6f || ( DateTime.UtcNow - d1 ).TotalSeconds < 0) {
                    IsLoading = true;
                    Debug.Log( "possible cheat detected" );
                }

                if (!this) return;
            } while (true);
        }

        async Task updateFromInternet() {
            try {
                // get time from time.nist.gov
                var client = new System.Net.Sockets.TcpClient( "time.nist.gov", 13 );
                var stream = client.GetStream();
                var reader = new System.IO.StreamReader( stream );
                var response = await reader.ReadToEndAsync();
                var utcTimeString = response.Substring( 7, 17 );
                var correctTime = DateTime.ParseExact( utcTimeString, "yy-MM-dd HH:mm:ss", null );
                _timeDifference = (float) (correctTime - DateTime.UtcNow).TotalSeconds;
                IsLoading = false;
                Debug.Log( $"time synced. diff: {_timeDifference}. utc: {DateTime.UtcNow.AddSeconds( _timeDifference ):T}" );
            }
            catch (Exception e) {
                Debug.LogError( e );
                IsLoading = true;
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