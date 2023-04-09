using System.Collections;
using TowerDefense.Ad;
using TowerDefense.Background.Loading;
using TowerDefense.Core.Env;
using TowerDefense.Player.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Background {
    public class GameInitializer : MonoBehaviour {
        
        IEnumerator Start() {
            
            LoadingScreenManager.Current.StartLoadingScreen();
            LoadingScreenManager.Current.state = LoadingScreenManager.State.StartingGame;
            
            // load ad
            if (Application.isEditor) new EditorAdManager();
            else new TapsellAdManager();
            bool success = false;
            yield return new WaitForTask<bool>( AdManager.Current.Initialize(), result => success = result );
            Debug.Log( success ? $"ad initalized successfully" : $"Ad failed to initialize" );
            
            // load data
            SecureDatabase.Current.Load();

            // load lobby
            var scenePath = SceneDatabase.Instance.GetScenePath( "lobby" );
            yield return SceneManager.LoadSceneAsync( scenePath, LoadSceneMode.Additive);
            yield return null;
            yield return null;
            SceneManager.SetActiveScene( SceneManager.GetSceneByPath( scenePath ) );
            
            LoadingScreenManager.Current.EndLoadingScreen();
        }
    }
}