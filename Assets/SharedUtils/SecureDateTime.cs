using System;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SomeDeveloper {
    /// <summary>
    /// Syncs time from internet and prevents time jump cheat. It automatically initializes itself and runs forever.
    /// you can tweak the settings in the code or by using the hooks.
    /// </summary>
    public static class SecureDateTime {
        
        const bool DEBUG_MODE = false;
        const int UPDATE_FROM_INTERNET_LOOP_DELAY = 3000;
        const bool LOG_TO_UNITY_CONSOLE = true;
        const bool AUTO_SYNC_RUN_FOREVER = true;
        const bool JUMP_CHEC_RUN_FOREVER = true;
        
        /// <summary> whether or not it's in the middle of resolving the time. could also mean there's an error
        /// or internet connectivity lost </summary>
        static public bool IsLoading { get; private set; } = true;

        /// <summary> returns synced <see cref="DateTime"/> </summary>
        public static DateTime GetSecureUtcDateTime() {
            return DateTime.UtcNow.AddSeconds( _timeDifference );
        }

        static float _timeDifference;
        
#region Hooks
        static public event Func<DateTime, DateTime> OnTimeDifferenceResolveFromInternet;
        static public event Action<string> OnLogMessage;
        static public event Action<Exception> OnLogError;
        static public event Action OnSyncFromInternet;
        static public event Action OnFaiilToSyncFromInternet;
#endregion
        

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        static void initialize() {
            if (AUTO_SYNC_RUN_FOREVER) syncTimeFromInterner_forever();
            if (JUMP_CHEC_RUN_FOREVER) timeJumpCheck_forever();
        }

        /// <summary> It'll run a check to see if the time jumps during the execution of this method. it takes 0.1 seconds.
        /// (this'll automatically be called if you leave <see cref="JUMP_CHEC_RUN_FOREVER"/> ON) </summary>
        static public async Task PerformTimeJumpCheck() {
            DateTime d1 = new(DateTime.UtcNow.Ticks); // get a copy of utc now
            await Task.Delay( 100 );
            if (( DateTime.UtcNow - d1 ).TotalSeconds > 0.6f || ( DateTime.UtcNow - d1 ).TotalSeconds < 0) {
                IsLoading = true;
                log( "possible cheat detected" );
            }
        }

        /// <summary> It'll sync the time from internet.
        /// (this'll automatically be called if you leave <see cref="AUTO_SYNC_RUN_FOREVER"/> ON) </summary>
        static public async Task PerformTimeSyncFromInternet() {
            try {
#if UNITY_EDITOR
                if (!Application.isPlaying) return;
#endif
                string response = string.Empty;

                bool done = false;
                
                // run on background thread. apparently ReadToEndAsync blocks main thread lol
            #pragma warning disable CS4014
                Task.Run( async () => {
                    // get time from time.nist.gov
                    using var client = new TcpClient( "time.nist.gov", 13 );
                    using var reader = new System.IO.StreamReader( client.GetStream() );
                    response = await reader.ReadToEndAsync();
                    done = true;
                } );
            #pragma warning restore CS4014
                
                while (!done) await Task.Yield();

                // check if it's a ok response
                if (response.StartsWith( "Access d" ) || response.Length < 25) {
                    OnFaiilToSyncFromInternet?.Invoke();
                    IsLoading = true;
                    return;
                }
                
                var utcTimeString = response.Substring( 7, 17 );
                
                if (DEBUG_MODE) {
                    // catching response on parse problem
                    if (!DateTime.TryParseExact( utcTimeString, "yy-MM-dd HH:mm:ss", null, DateTimeStyles.AllowWhiteSpaces, out var _ ))
                        log( $"couldn't parse string \"{utcTimeString}\" from {response}" );
                }
                
                var correctTime = DateTime.ParseExact( utcTimeString, "yy-MM-dd HH:mm:ss", null );

                if (OnTimeDifferenceResolveFromInternet is not null) {
                    try { correctTime = OnTimeDifferenceResolveFromInternet( correctTime ); }
                    catch (Exception e) { logExp( e ); }
                }
                
                _timeDifference = (float)( correctTime - DateTime.UtcNow ).TotalSeconds;
                IsLoading = false;
                
                try { OnSyncFromInternet?.Invoke(); }
                catch (Exception e) { logExp( e ); }

                if (DEBUG_MODE)
                    log( $"time synced. diff: {_timeDifference}. utc: {DateTime.UtcNow.AddSeconds( _timeDifference ):T}" );
            }
            catch (Exception e) {
                logExp( e );
                OnFaiilToSyncFromInternet?.Invoke();
                IsLoading = true;
            }
        }

        
        static async void timeJumpCheck_forever() {
            do {
#if UNITY_EDITOR
                if (Application.isPlaying) {
                    await PerformTimeJumpCheck();
                }
                else {
                    await Task.Delay( 200 ); // waiting for possibility of entering playmode
                }
#else
                await PerformTimeJumpCheck();
#endif

            } while (true);
        }

        static async void syncTimeFromInterner_forever() {
            do {
#if UNITY_EDITOR
                if (Application.isPlaying) {
                    await PerformTimeSyncFromInternet();
                }
#else
                await PerformTimeSyncFromInternet();
#endif
                await Task.Delay( UPDATE_FROM_INTERNET_LOOP_DELAY );
            } while (true);
        }

        static void log(string message) {
            if (LOG_TO_UNITY_CONSOLE) Debug.Log( message );
            OnLogMessage?.Invoke(message);
        }
        
        static void logExp(Exception message) {
            if (LOG_TO_UNITY_CONSOLE) Debug.LogException( message );
            OnLogError?.Invoke(message);
        }
    }
}