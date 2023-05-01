using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimFlex;
using AnimFlex.Core.Proxy;
using AnimFlex.Tweening;
using DialogueSystem;
using GameAnalyticsSDK;
using TowerDefense.Background;
using TowerDefense.Background.Loading;
using TowerDefense.Bridges.Analytics;
using TowerDefense.Core.Enemies;
using TowerDefense.Core.Defenders;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using TowerDefense.Core.Road;
using TowerDefense.Core.Starter;
using TowerDefense.Core.UI;
using TowerDefense.Core.UI.Lose;
using TowerDefense.Core.UI.Win;
using TowerDefense.Data;
using TowerDefense.Data.Database;
using TowerDefense.Data.Progress;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Core {
    public class CoreGameManager : MonoBehaviour {

        public static CoreGameManager Current;

        public RoadManager roadManager;
        public int life = 20;
        public float gameTime = 0;
        public AudioSource backgroundMusicSource;
        
        [SerializeField] float winDialogueDelay = 1;
        [SerializeField] float loseDialogueDelay = 1;

        [Title("Runtime")]
        [ShowInInspector] public readonly List<Enemy> enemies = new(16);
        [ShowInInspector] public readonly List<Defender> defenders = new(8);
        [ShowInInspector] public readonly List<EnemySpawner> spawners = new(4);
        [ShowInInspector] public CoreSessionPack sessionPack;
        
        CoreLevelData _levelData;
        bool _gameActive = false;
        ulong _coinsReceivedFromEnemiesKilled = 0;
        Tweener _slowDownTweener = null;
        


        void OnEnable() {
            CoreGameEvents.Current.OnStartupFinished += OnCoreStarterFinished;
            Current = this;
        }
        
        void Start() {
            fadeInMusic();
            gameTime = 0;
            _gameActive = true;
            CoreGameEvents.Current.OnGameStart?.Invoke( this );
            CoreGameEvents.Current.OnEnemySpawn += OnEnemySpawn;
            CoreGameEvents.Current.OnEnemyReachEnd += OnEnemyReachEnd;
            CoreGameEvents.Current.OnEnemyDestroy += OnEnemyDestroy;
            CoreGameEvents.Current.OnDefenderDestroy += OnDefenderDestroy;
            CoreGameEvents.Current.OnDefenderSpawn += OnDefenderSpawn;
            CoreGameEvents.Current.OnEnemySpawnerInitialize += OnSpawnerInitialize;
            CoreGameEvents.Current.onSessionCoinModified?.Invoke();
            CoreGameEvents.Current.onLifeModified?.Invoke();

            try {
                GameAnalytics.NewProgressionEvent( GAProgressionStatus.Start, _levelData.id );
            }
            catch {
                // ignored
            }
        }

        void OnDestroy() {
            CoreGameEvents.Current.OnStartupFinished -= OnCoreStarterFinished;
            CoreGameEvents.Current.OnEnemySpawn -= OnEnemySpawn;
            CoreGameEvents.Current.OnEnemyReachEnd -= OnEnemyReachEnd;
            CoreGameEvents.Current.OnEnemyDestroy -= OnEnemyDestroy;
            CoreGameEvents.Current.OnDefenderDestroy -= OnDefenderDestroy;
            CoreGameEvents.Current.OnDefenderSpawn -= OnDefenderSpawn;
            CoreGameEvents.Current.OnEnemySpawnerInitialize -= OnSpawnerInitialize;
            if (_slowDownTweener is not null && _slowDownTweener.IsActive()) {
                _slowDownTweener.Kill( false, false );
            }
            if (Time.timeScale != 1) {
                Time.timeScale = 1;
            }
        }

        void Update() {
            if (_gameActive) {
                gameTime += Time.deltaTime;
                CoreGameEvents.Current.onTimeModified?.Invoke();
            }
        }

        public void RestartGame() {
            BackgroundRunner.Current.StartCoroutine( coroutine( _levelData ) );

            IEnumerator coroutine(CoreLevelData coreStarter) {
                LoadingScreenManager.Current.StartLoadingScreen();
                LoadingScreenManager.Current.state = LoadingScreenManager.State.RestartingCore;
                
                yield return null;
                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                bool canContinue = false;
                BackgroundRunner.Current.StartCoroutine( CoreStartup.StartCore( _levelData, sessionPack, () => canContinue = true ) );
                yield return new WaitUntil( () => canContinue );
                LoadingScreenManager.Current.EndLoadingScreen();
            }
        }

        public void BackToLobby() {
            BackgroundRunner.Current.StartCoroutine( enumerator() );

            IEnumerator enumerator() {
                LoadingScreenManager.Current.StartLoadingScreen();
                LoadingScreenManager.Current.state = LoadingScreenManager.State.BackFromCore;
                yield return SceneManager.LoadSceneAsync( SceneDatabase.Instance.GetScenePath( "lobby" ), LoadSceneMode.Additive );
                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                LoadingScreenManager.Current.EndLoadingScreen();
            }
        }


        void OnCoreStarterFinished(CoreLevelData coreLevelData, CoreSessionPack sessionPack) {
            _levelData = Instantiate( coreLevelData );
            this.sessionPack = sessionPack;
        }

        void OnSpawnerInitialize(EnemySpawner spawner) {
            spawners.Add( spawner );
        }

        void OnDefenderSpawn(Defender defender) {
            defenders.Add( defender );
        }

        void OnDefenderDestroy(Defender defender) {
            Destroy( defender.gameObject );
        }

        void OnEnemySpawn(Enemy enemy) {
            enemies.Add( enemy );
        }

        void OnEnemyDestroy(Enemy enemy) {
            _coinsReceivedFromEnemiesKilled += enemy.coinReward;
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            if (checkForWin()) Win();
        }

        void OnEnemyReachEnd(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            life -= 1;
            CoreGameEvents.Current.onLifeModified?.Invoke();
            if (checkForLose()) Lose();
            if (checkForWin()) Win();
        }

        void fadeInMusic() => backgroundMusicSource.AnimAudioVolumeTo( 1 );

        void fadeOutMusic() => backgroundMusicSource.AnimAudioVolumeTo( 0, proxy: AnimFlexCoreProxyUnscaled.Default );

        bool checkForLose() {
            if (life <= 0 && _gameActive) {
                _gameActive = false;
                return true;
            }

            return false;
        }

        bool checkForWin() {
            if (enemies.Count == 0 && spawners.All( s => s.IsDone() )) {
                return true;
            }
            return false;
        }

        async void Win() {
            fadeOutMusic();
            
            // making data for win
            var winData = new WinData();
            winData.time = gameTime;
            winData.stars = _levelData.EvaluateStar( gameTime );
            winData.coins = (ulong)( _coinsReceivedFromEnemiesKilled * _levelData.coinMultiplier )
                            + (ulong)winData.stars * _levelData.EvaluateBonusCoinForStar( winData.stars );
            Debug.Log( $"Won Game!: {winData.ToJson()}" );


            GameAnalytics.NewResourceEvent( GAResourceFlowType.Source, GameAnalyticsHelper.Currency_Coin, winData.coins,
                GameAnalyticsHelper.ItemType_GameWin, "GameWin" );
            GameAnalytics.NewProgressionEvent( GAProgressionStatus.Complete, _levelData.id );
            
            CoreGameEvents.Current.onWin?.Invoke( winData );

            // handle data modification
            var level = PlayerGlobals.Current.GetOrCreateLevelProg( _levelData.id );
            level.status |= LevelProgress.LevelStatus.Finished;
            level.stars = winData.stars;
            level.coinsReceived += winData.coins;
            level.playCount++;
            // check if there's next level, if so unlock it 
            if (PlayerGlobals.Current.TryGetNextLevelProg( level.id, out var nextLevel )) {
                nextLevel.status |= LevelProgress.LevelStatus.Unlocked; 
            }
            PlayerGlobals.Current.ecoProg.coins += winData.coins;
            PlayerGlobals.Current.Save( SecureDatabase.Current );
            
            // slow down time
            _slowDownTweener = Tweener.Generate( () => Time.timeScale, value => Time.timeScale = value, 0f,
                winDialogueDelay, proxy: AnimFlexCoreProxyUnscaled.Default );
            
            // open win dialogue
            await Task.Delay( (int)(winDialogueDelay * 1000) );
            var dialogue =
                DialogueManager.Current.GetOrCreate<WinDialogue>( parentTransform: CoreUI.Current.transform );
            dialogue.winData = winData;
            
        }

        async void Lose() {
            fadeOutMusic();
            
            // making data for lose 
            var loseData = new LoseData();
            loseData.time = gameTime;
            loseData.coins = (ulong)( _coinsReceivedFromEnemiesKilled * _levelData.coinMultiplier );
            Debug.Log( $"Won Game!: {loseData.ToJson()}" );
            CoreGameEvents.Current.onLose?.Invoke(loseData);

            GameAnalytics.NewResourceEvent( GAResourceFlowType.Sink, GameAnalyticsHelper.Currency_Coin,
                sessionPack.coins, GameAnalyticsHelper.ItemType_GameLose, "GameLose" );
            GameAnalytics.NewProgressionEvent( GAProgressionStatus.Fail, _levelData.id );
            
            // handle data modification
            var level = PlayerGlobals.Current.GetOrCreateLevelProg( _levelData.id );
            level.playCount++;
            level.coinsReceived += loseData.coins;
            PlayerGlobals.Current.ecoProg.coins += loseData.coins;
            PlayerGlobals.Current.Save( SecureDatabase.Current );
            
            // slow down time
            _slowDownTweener = Tweener.Generate( () => Time.timeScale, value => Time.timeScale = value, 0f, loseDialogueDelay,
                proxy: AnimFlexCoreProxyUnscaled.Default );

            // open lose dialogue
            await Task.Delay( (int)(loseDialogueDelay * 1000) );
            var dialogue = DialogueManager.Current.GetOrCreate<LoseDialogue>( parentTransform: CoreUI.Current.transform );
            dialogue.loseData = loseData;
        }
    }
}