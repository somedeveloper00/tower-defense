using TowerDefense.Data.Database;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    public class Tests : MonoBehaviour {
        public Button deleteAll;

        void Start() {
            deleteAll?.onClick.AddListener( deleteData );
        }

        public void deleteData() {
            SecureDatabase.Current.DeleteAll();
            PreferencesDatabase.Current.DeleteAll();
            SecureDatabase.Current.Load();
            PreferencesDatabase.Current.Load();
            SceneManager.LoadScene( SceneManager.GetActiveScene().name, LoadSceneMode.Additive );
            SceneManager.UnloadScene( gameObject.scene );
        }
    }
}