using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DialogueSystem;
using TowerDefense.Background;
using TowerDefense.Background.Loading;
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
        public float life = 20;
        
        [SerializeField] float winDialogueDelay = 1;
        [SerializeField] float loseDialogueDelay = 1;

        [ShowInInspector, ReadOnly] public readonly List<Enemy> enemies = new(16);
        [ShowInInspector, ReadOnly] public readonly List<Defender> defenders = new(8);
        [ShowInInspector, ReadOnly] public readonly List<EnemySpawner> spawners = new(4);

        CoreLevelData _levelData;
        bool _gameActive = false;
        float gameTime = 0;
        
        void OnEnable() {
            CoreGameEvents.Current.OnStartupFinished += OnCoreStarterFinished;
            Current = this;
        }
        
        void Start() {
            gameTime = 0;
            _gameActive = true;
            CoreGameEvents.Current.OnGameStart?.Invoke( this );
            CoreGameEvents.Current.OnEnemySpawn += OnEnemySpawn;
            CoreGameEvents.Current.OnEnemyReachEnd += OnEnemyReachEnd;
            CoreGameEvents.Current.OnEnemyDestroy += OnEnemyDestroy;
            CoreGameEvents.Current.OnDefenderDestroy += OnDefenderDestroy;
            CoreGameEvents.Current.OnDefenderSpawn += OnDefenderSpawn;
            CoreGameEvents.Current.OnEnemySpawnerInitialize += OnSpawnerInitialize;
        }

        void OnDestroy() {
            CoreGameEvents.Current.OnStartupFinished -= OnCoreStarterFinished;
            CoreGameEvents.Current.OnEnemySpawn -= OnEnemySpawn;
            CoreGameEvents.Current.OnEnemyReachEnd -= OnEnemyReachEnd;
            CoreGameEvents.Current.OnEnemyDestroy -= OnEnemyDestroy;
            CoreGameEvents.Current.OnDefenderDestroy -= OnDefenderDestroy;
            CoreGameEvents.Current.OnDefenderSpawn -= OnDefenderSpawn;
            CoreGameEvents.Current.OnEnemySpawnerInitialize -= OnSpawnerInitialize;
        }

        void Update() {
            if (_gameActive) {
                gameTime += Time.deltaTime;
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
                BackgroundRunner.Current.StartCoroutine( CoreStartup.StartCore( _levelData, () => canContinue = true ) );
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
        
        
        void OnCoreStarterFinished(CoreLevelData coreLevelData) {
            _levelData = Instantiate( coreLevelData );
        }

        void OnSpawnerInitialize(EnemySpawner spawner) {
            spawners.Add( spawner );
        }

        void OnDefenderSpawn(Defender defender) {
            defender.coreGameManager = this;
            defenders.Add( defender );
        }

        void OnDefenderDestroy(Defender defender) {
            Destroy( defender.gameObject );
        }

        void OnEnemySpawn(Enemy enemy) {
            enemies.Add( enemy );
        }

        void OnEnemyDestroy(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            // check if any spawners has any enemy to spawn
            if (enemies.Count == 0 && spawners.All( s => s.IsDone() )) {
                Win();
            }
        }

        void OnEnemyReachEnd(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            life -= 1;
            if (life <= 0 && _gameActive) {
                _gameActive = false;
                Debug.Log( $"Lost Game!" );
                Lose();
            }
        }
        
        async void Win() {
            // making data for win
            var winData = new WinData();
            winData.time = gameTime;
            winData.stars = gameTime > _levelData.starTime[2] ? 3 :
                gameTime > _levelData.starTime[1] ? 2 :
                gameTime > _levelData.starTime[0] ? 1 : 0;
            Debug.Log( $"Won Game!: {winData.ToJson()}" );

            CoreGameEvents.Current.onWin?.Invoke( winData );
            

            // handle data modification
            var level = PlayerGlobals.Current.GetOrCreateLevelProg( _levelData.id );
            level.status |= LevelProgress.LevelStatus.Finished;
            level.stars = 3;
            level.playCount++;
            // check if there's next level, if so unlock it 
            if (PlayerGlobals.Current.TryGetNextLevelProg( level.id, out var nextLevel )) {
                nextLevel.status |= LevelProgress.LevelStatus.Unlocked; 
            }
            PlayerGlobals.Current.Save( SecureDatabase.Current );
            
            // open win dialogue
            await Task.Delay( (int)(winDialogueDelay * 1000) );
            var dialogue =
                DialogueManager.Current.GetOrCreate<WinDialogue>( parentTransform: CoreUI.Current.transform );
            dialogue.winData = winData;
        }

        async void Lose() {
            await Task.Delay( (int)(loseDialogueDelay * 1000) );
            var dialogue =
                DialogueManager.Current.GetOrCreate<LoseDialogue>( parentTransform: CoreUI.Current.transform );
            
            // handle data modification
            var level = PlayerGlobals.Current.GetOrCreateLevelProg( _levelData.id );
            level.playCount++;
            PlayerGlobals.Current.Save( SecureDatabase.Current );
        }
    }
}