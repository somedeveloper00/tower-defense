using System.IO;
using DialogueSystem;
using TowerDefense.Bridges.Analytics;
using TowerDefense.Data.Database;
using TowerDefense.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using TriInspector;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    public class Tests : MonoBehaviour {
        public Button deleteAll;
        public Button setSaveData1;
        
        [TextArea]
        [SerializeField] string saveData1;

        void Start() {
            deleteAll?.onClick.AddListener( deleteData );
            setSaveData1?.onClick.AddListener( SetSaveData1 );
        }

        void SetSaveData1() {
            SecureDatabase.Current.Save();
            File.WriteAllText( Path.Combine( Application.persistentDataPath, "sv1.dat" ), saveData1 );
            new SecureDatabase( "sv1.dat" );
            SecureDatabase.Current.Load();
            SceneManager.LoadScene( SceneManager.GetActiveScene().name, LoadSceneMode.Additive );
            SceneManager.UnloadScene( gameObject.scene );
        }

        public void deleteData() {
            SecureDatabase.Current.DeleteAll();
            PreferencesDatabase.Current.DeleteAll();
            SecureDatabase.Current.Load();
            PreferencesDatabase.Current.Load();
            SceneManager.LoadScene( SceneManager.GetActiveScene().name, LoadSceneMode.Additive );
            SceneManager.UnloadScene( gameObject.scene );
        }

        [Button]
        void openReward() {
            var d = DialogueManager.Current.GetOrCreate<RewardDialogue>( );
            d.coins = 100;
            d.useCustomCoinDisplayTarget = true;
            d.coinDisplayTarget = LobbyManager.Current.coinDisplay;
            d.itemType = GameAnalyticsHelper.ItemType.ItemType_Ad;
            d.showSparkles = true;
            d.showCoinShower = true;
        }
    }
}   