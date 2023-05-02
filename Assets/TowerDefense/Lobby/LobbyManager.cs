using System;
using System.Collections;
using System.Threading.Tasks;
using AnimFlex;
using AnimFlex.Sequencer.UserEnd;
using AnimFlex.Tweening;
using DialogueSystem;
using GameAnalyticsSDK;
using TowerDefense.Background;
using TowerDefense.Background.Loading;
using TowerDefense.Bridges.Ad;
using TowerDefense.Bridges.Analytics;
using TowerDefense.Common;
using TowerDefense.Core.Starter;
using TowerDefense.Data;
using TowerDefense.Lobby.LevelChoosing;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TowerDefense.Lobby {
    [DeclareFoldoutGroup( "ref", Title = "References", Expanded = true )]
    [DeclareFoldoutGroup( "ad", Title = "Ad params", Expanded = false )]
    public class LobbyManager : MonoBehaviour {

        public const string SidedBanner_AdId = "6432787d2eeae447e5ae5de3";
        public const string FullScreenVideoAd_AdId = "642ee1bd2eeae447e5ae5bb3";
        public const string FullScreenBannerAd_AdId = "642ff0e183b1d13d3401721e";

        public static LobbyManager Current;
        void OnEnable() => Current = this;

        [GroupNext( "ref" )] 
        [SerializeField] GraphicRaycaster raycaster;
        [SerializeField] RectTransform parentCanvasForDialogues;
        [SerializeField] SequenceAnim inSequence;
        [SerializeField] Button playBtn;
        [SerializeField] Button exitBtn;
        [SerializeField] Button shopBtn;
        [SerializeField] Button settingsBtn;
        public AudioSource backgroundMusicSource;

        void Start() {
            // fade in background music
            backgroundMusicSource.AnimAudioVolumeTo( 1 );
            
            playBtn.onClick.AddListener( onPlayBtnClick );
            exitBtn.onClick.AddListener( onExitBtnClick );
            shopBtn.onClick.AddListener( onShopBtnClick );
            settingsBtn.onClick.AddListener( onSettingsBtnClick );
            inSequence.sequence.onComplete += () => {
                raycaster.enabled = true;
                Debug.Log( $"in seq finished" );
            };
            raycaster.enabled = false;

            if (LoadingScreenManager.Current != null && LoadingScreenManager.Current.IsON()) {
                // ReSharper disable once AsyncVoidLambda
                LoadingScreenManager.Current.onEndAnimStartOnce += async () => {
                    if (LoadingScreenManager.Current.state == LoadingScreenManager.State.StartingGame) {
                        inSequence.PlaySequence();
                    }
                    else if (LoadingScreenManager.Current.state == LoadingScreenManager.State.BackFromCore) {
                        if (Random.Range( 0, 2 ) == 0) await ShowFullScreenBannerAd();
                        else await ShowFullScreenBannerAd();
                        inSequence.PlaySequence();
                    }
                    await ShowSidedBannerAd();
                };
            }
            else {
                inSequence.PlaySequence();
            }
        }

        void OnDestroy() {
            removeBannerAsync();
            
            async void removeBannerAsync() {
                while (await IsShowingSidedBannerAd()) 
                    await RemoveSidedBannerAd();
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
            DialogueManager.Current.GetOrCreate<ShopDialogue.ShopDialogue>( parentCanvasForDialogues );
        }

        void onSettingsBtnClick() { }

        public void StartGame(CoreLevelData levelDataObject, CoreSessionPack sessionPack) {

            if (PlayerGlobals.Current.ecoProg.coins < sessionPack.coins) {
                throw new Exception( "not enough coins" );
            }

            PlayerGlobals.Current.ecoProg.coins -= sessionPack.coins;
            if (PlayerGlobals.Current.ecoProg.coins < 20) PlayerGlobals.Current.ecoProg.coins = 20;
            
            GameAnalytics.NewResourceEvent( GAResourceFlowType.Sink, GameAnalyticsHelper.Currency_Coin,
                sessionPack.coins, GameAnalyticsHelper.ItemType_GameStart, "GameStart" );

            BackgroundRunner.Current.StartCoroutine( start() );

            IEnumerator start() {
                LoadingScreenManager.Current.StartLoadingScreen();
                LoadingScreenManager.Current.state = LoadingScreenManager.State.GoingToCore;
                yield return null;

                // fade out background music
                var t = backgroundMusicSource.AnimAudioVolumeTo( 0, duration: 1 );
                while (t.IsActive()) yield return new WaitForSecondsRealtime( 1 );
                
                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                yield return null;

                BackgroundRunner.Current.StartCoroutine( CoreStartup.StartCore( levelDataObject, sessionPack,
                    onComplete: () => { BackgroundRunner.Current.StartCoroutine( onComplete() ); } ) );
            }

            IEnumerator onComplete() {
                yield return null;
                LoadingScreenManager.Current.EndLoadingScreen();
                Debug.Log( $"core game starting finished." );
            }
        }

        public async Task ShowFullScreenBannerAd() {
            var loadingDialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>( parentCanvasForDialogues );
            loadingDialogue.UsePresetForLoadingAd();

            raycaster.enabled = false;
            await AdManager.Current.ShowFullScreenBannerAd( FullScreenBannerAd_AdId );
            Debug.Log( $"finished fullscreen ad" );
            await loadingDialogue.Close();
        }

        public async Task ShowFullScreenVideoAd() {
            var loadingDialogue = DialogueManager.Current.GetOrCreate<MessageDialogue>( parentCanvasForDialogues );
            loadingDialogue.UsePresetForLoadingAd();

            raycaster.enabled = false;
            await AdManager.Current.ShowFullScreenVideoAd( FullScreenVideoAd_AdId );
            Debug.Log( $"finished fullscreen vid ad" );
            await loadingDialogue.Close();
        }

        public async Task ShowSidedBannerAd() {
            await AdManager.Current.ShowSidedBannerAd( SidedBanner_AdId );
            Debug.Log( $"showed sided banner ad" );
        }

        public async Task<bool> IsShowingSidedBannerAd() => await AdManager.Current.IsSidedBannerAdShowing( SidedBanner_AdId );
        
        public async Task RemoveSidedBannerAd() {
            await AdManager.Current.RemoveSidedBannerAd( SidedBanner_AdId );
            Debug.Log( $"removed sided banner ad" );
        }
    }
}