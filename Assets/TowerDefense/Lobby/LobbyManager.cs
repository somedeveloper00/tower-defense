using System;
using System.Collections;
using System.Threading;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TowerDefense.Background;
using TowerDefense.Background.Loading;
using TowerDefense.Core;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using TowerDefense.Core.Starter;
using TowerDefense.Lobby.LevelChoosing;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    public class LobbyManager : MonoBehaviour {

        public static LobbyManager Current;
        void OnEnable() => Current = this;

        public bool useJson = true;

        [GroupNext( "ref" )] 
        [SerializeField] RectTransform parentCanvasForDialogues;
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
            // StartGame();
            DialogueManager.Current.GetOrCreate<LevelChoosingDialogue>( parentTransform: parentCanvasForDialogues );
        }
        void onExitBtnClick() {
            Application.Quit();
        }
        void onShopBtnClick() {
            
        }
        void onSettingsBtnClick() {
            
        }
        
        public void StartGame(CoreLevelData levelDataObject, string levelDataJson) {

            BackgroundRunner.Current.StartCoroutine( start() );

            IEnumerator start() {
                LoadingScreenManager.Current.StartLoadingScreen();
                yield return null;
                
                // handle josn to object
                if (!levelDataObject) {
                    var level = ScriptableObject.CreateInstance<CoreLevelData>();
                    var thread = new Thread( () => level.FromJson( levelDataJson ) );
                    thread.IsBackground = true;
                    thread.Start();
                    yield return new WaitUntil( () => thread.IsAlive == false );
                }

                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                yield return null;
                BackgroundRunner.Current.StartCoroutine( CoreStartup.StartCore( levelDataObject, onComplete: () => {
                    BackgroundRunner.Current.StartCoroutine( onComplete() );
                } ) );
            }

            IEnumerator onComplete() {
                yield return null;
                LoadingScreenManager.Current.EndLoadingScreen();
                Debug.Log( $"core game starting finished." );
            }
        }
        


    }
}