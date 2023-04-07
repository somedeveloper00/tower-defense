using System;
using System.Collections;
using System.Threading;
using AnimFlex.Sequencer.UserEnd;
using DialogueSystem;
using TowerDefense.Ad;
using TowerDefense.Background;
using TowerDefense.Background.Loading;
using TowerDefense.Common;
using TowerDefense.Core.Starter;
using TowerDefense.Lobby.LevelChoosing;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TowerDefense.Lobby {
    [DeclareFoldoutGroup("ref", Title = "References", Expanded = true)]
    [DeclareFoldoutGroup("ad", Title = "Ad params", Expanded = false)]
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

        [GroupNext( "ad" )] 
        [SerializeField] string[] loadingAdText;

        void Start() {
            playBtn.onClick.AddListener( onPlayBtnClick );
            exitBtn.onClick.AddListener( onExitBtnClick );
            shopBtn.onClick.AddListener( onShopBtnClick );
            settingsBtn.onClick.AddListener( onSettingsBtnClick );
            if (LoadingScreenManager.Current.IsON()) {
                LoadingScreenManager.Current.onEndAnimStart += () => {
                    if (LoadingScreenManager.Current.state == LoadingScreenManager.State.StartingGame) {
                        ShowFullScreenBannerAd();
                    }
                    else {
                        ShowFullScreenVideoAd();
                    }
                };
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
                LoadingScreenManager.Current.state = LoadingScreenManager.State.GoingToCore;
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

        [Button]
        public async void ShowFullScreenBannerAd() {
            var loadingDialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>( parentCanvasForDialogues );
            loadingDialogue.UsePresetForLoadingAd();
            
            await AdManager.Current.ShowFullScreenBannerAd( "642ff0e183b1d13d3401721e" );
            inSequence.PlaySequence();
            await loadingDialogue.Close();
        }
        [Button]
        public async void ShowFullScreenVideoAd() {
            var loadingDialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>( parentCanvasForDialogues );
            loadingDialogue.UsePresetForLoadingAd();
            
            await AdManager.Current.ShowFullScreenVideoAd( "642ee1bd2eeae447e5ae5bb3" );
            inSequence.PlaySequence();
            await loadingDialogue.Close();
        }

    }
}