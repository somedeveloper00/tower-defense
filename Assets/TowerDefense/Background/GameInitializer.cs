using System;
using System.Collections;
using TowerDefense.Background.Loading;
using TowerDefense.Bridges.Ad;
using TowerDefense.Bridges.Iap;
using TowerDefense.Core.Env;
using TowerDefense.Data.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Background {
    public class GameInitializer : MonoBehaviour {
        
        public static Action<SecureDatabase> onSecureDataLoad;
        
        IEnumerator Start() {
            
            LoadingScreenManager.Current.StartLoadingScreen();
            LoadingScreenManager.Current.state = LoadingScreenManager.State.StartingGame;
            
            // load ad
            if (Application.isEditor) new EditorAdManager();
            else new TapsellAdManager();
            bool success = false;
            yield return new WaitForTask<bool>( AdManager.Current.Initialize(), result => success = result );
            Debug.Log( success ? $"ad initalized successfully" : $"Ad failed to initialize" );
            
            // load in app purchasing
            var iapManager = new BazaarInAppPurchase();
            yield return new WaitForTask( iapManager.Initialize() );
            
            // load data
            onSecureDataLoad?.Invoke( SecureDatabase.Current );

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