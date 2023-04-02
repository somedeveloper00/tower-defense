using System.Collections;
using TowerDefense.Background.Loading;
using TowerDefense.Core.Env;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Background {
    public class LoadLobbyOnStart : MonoBehaviour {
        [SerializeField] bool destroySelf = true;
        IEnumerator Start() {
            LoadingScreenManager.Current.StartLoadingScreen();
            
            var scenePath = SceneDatabase.Instance.GetScenePath( "lobby" );
            yield return SceneManager.LoadSceneAsync( scenePath, LoadSceneMode.Additive);
            yield return null;
            yield return null;
            SceneManager.SetActiveScene( SceneManager.GetSceneByPath( scenePath ) );
            
            LoadingScreenManager.Current.EndLoadingScreen();
            if (destroySelf) Destroy( gameObject );
        }
    }
}