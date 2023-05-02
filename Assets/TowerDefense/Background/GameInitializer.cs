using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameAnalyticsSDK;
using TowerDefense.Background.Loading;
using TowerDefense.Bridges.Ad;
using TowerDefense.Bridges.Iap;
using TowerDefense.Core.Env;
using TowerDefense.Data.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Background {
    public class GameInitializer : MonoBehaviour {
        
        public static Action<SecureDatabase> afterSecureDataLoad;
        public static Action<SecureDatabase> beforeSecureDataSave;
        public List<OnInitTask> onInitTasks = new List<OnInitTask>();

        public class OnInitTask {
            public int order;
            public Task task;

            public OnInitTask(int order, Task task) {
                this.order = order;
                this.task = task;
            }
        }

        public static GameInitializer Current;
        void OnEnable() => Current = this;
        
        IEnumerator Start() {
            
            LoadingScreenManager.Current.StartLoadingScreen();
            LoadingScreenManager.Current.state = LoadingScreenManager.State.StartingGame;

            // load data
            new SecureDatabase( "s.dat" );
            SecureDatabase.Current.Load();
            afterSecureDataLoad?.Invoke( SecureDatabase.Current );
            
            // save data right after loading for flushing save file
            beforeSecureDataSave?.Invoke( SecureDatabase.Current );
            SecureDatabase.Current.Save();
            
            // load ad
            if (Application.isEditor) new EditorAdManager();
            else new TapsellAdManager();
            bool success = false;
            yield return new WaitForTask<bool>( AdManager.Current.Initialize(), result => success = result );
            Debug.Log( success ? $"ad initalized successfully" : $"Ad failed to initialize" );
            
            // load in app purchasing
            var iapManager = new BazaarInAppPurchase();
            yield return new WaitForTask( iapManager.InitializeIfNotAlready() );
            
            // load analytics
            GameAnalytics.Initialize();

            // do after load tasks
            onInitTasks.Sort( (t1, t2) => t1.order.CompareTo( t2.order ) );
            for (int i = 0; i < onInitTasks.Count; i++) {
                yield return new WaitForTask( onInitTasks[i].task );
            }

            // load lobby
            var scenePath = SceneDatabase.Instance.GetScenePath( "lobby" );
            yield return SceneManager.LoadSceneAsync( scenePath, LoadSceneMode.Additive);
            yield return null;
            yield return null;
            SceneManager.SetActiveScene( SceneManager.GetSceneByPath( scenePath ) );
            
            LoadingScreenManager.Current.EndLoadingScreen();
        }

        void OnApplicationQuit() {
            InAppPurchase.Current.Close();
        }
        
        
    }
}