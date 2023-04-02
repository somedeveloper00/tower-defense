using System.Collections;
using System.Threading;
using AnimFlex.Sequencer.UserEnd;
using TowerDefense.Background;
using TowerDefense.Background.Loading;
using TowerDefense.Core.Starter;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class LobbyManager : MonoBehaviour {

        public bool useJson = true;

        [ShowIf( nameof(useJson), false )] public CoreStarter coreStarter;
        [ShowIf( nameof(useJson), true ), TextArea] public string json;
        [GroupNext( "ref" )] 
        [SerializeField] SequenceAnim inSequence;
        [SerializeField] Button playBtn;
        [SerializeField] Button exitBtn;
        [SerializeField] Button shopBtn;
        [SerializeField] Button settingsBtn;

        void Start() {
            playBtn.onClick.AddListener( onPlayBtnClick );
            exitBtn.onClick.AddListener( onExitBtnClick );
            shopBtn.onClick.AddListener( onShopBtnClick );
            settingsBtn.onClick.AddListener( onSettingsBtnClick );
            if (LoadingScreenManager.Current.IsON()) {
                LoadingScreenManager.Current.onEndAnimStart += inSequence.PlaySequence;
            }
            else {
                inSequence.PlaySequence();
            }
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

            BackgroundRunner.Current.StartCoroutine( start() );

            IEnumerator start() {
                LoadingScreenManager.Current.StartLoadingScreen();
                yield return null;
                
                // handle josn to object
                if (useJson) {
                    coreStarter = ScriptableObject.CreateInstance<CoreStarter>();
                    var thread = new Thread( setupCoreStarter );
                    thread.IsBackground = true;
                    thread.Start();
                    yield return new WaitUntil( () => thread.IsAlive == false );
                }

                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                yield return null;
                BackgroundRunner.Current.StartCoroutine(
                    coreStarter.StartGame( () => BackgroundRunner.Current.StartCoroutine( onComplete() ) ) );
            }

            IEnumerator onComplete() {
                yield return null;
                Debug.Log( $"lobby scene unloaded fully" );
                LoadingScreenManager.Current.EndLoadingScreen();
            }

            void setupCoreStarter() {
                coreStarter.FromJson( json );
            }
        }
    }
}