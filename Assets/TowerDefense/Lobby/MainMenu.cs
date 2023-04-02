using System.Collections;
using TowerDefense.Core.Starter;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class MainMenu : MonoBehaviour {

        public bool useJson = true;

        [ShowIf( nameof(useJson), false )] public CoreStarter coreStarter;
        [ShowIf( nameof(useJson), true ), TextArea] public string json;
        [GroupNext( "ref" )] 
        [SerializeField] Button playBtn;
        [SerializeField] Button exitBtn;
        [SerializeField] Button shopBtn;
        [SerializeField] Button settingsBtn;

        void Start() {
            playBtn.onClick.AddListener( onPlayBtnClick );
            exitBtn.onClick.AddListener( onExitBtnClick );
            shopBtn.onClick.AddListener( onShopBtnClick );
            settingsBtn.onClick.AddListener( onSettingsBtnClick );
        }
        
        void onPlayBtnClick() {
            StartGame();
        }
        void onExitBtnClick() {
            Application.Quit();
        }
        void onShopBtnClick() {
            
        }
        void onSettingsBtnClick() {
            
        }
        
        [Button(ButtonSizes.Large)]
        public void StartGame() {
            if (useJson) {
                coreStarter = ScriptableObject.CreateInstance<CoreStarter>();
                coreStarter.FromJson( json );
            }

            StartCoroutine( coreStarter.StartGame( () => StartCoroutine( onComplete() ) ) );

            IEnumerator onComplete() {
                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                Debug.Log( $"lobby scene unloaded" );
            }
        }
    }
}