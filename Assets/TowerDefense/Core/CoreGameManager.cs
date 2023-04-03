using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimFlex.Tweening;
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

        CoreStarter _starter;
        bool _lost = false;
        
        void OnEnable() {
            CoreGameEvents.Current.OnCoreStarterFinished += OnCoreStarterFinished;
            Current = this;
        }


        void Start() {
            CoreGameEvents.Current.OnGameStart?.Invoke( this );
            CoreGameEvents.Current.OnEnemySpawn += OnEnemySpawn;
            CoreGameEvents.Current.OnEnemyReachEnd += OnEnemyReachEnd;
            CoreGameEvents.Current.OnEnemyDestroy += OnEnemyDestroy;
            CoreGameEvents.Current.OnDefenderDestroy += OnDefenderDestroy;
            CoreGameEvents.Current.OnDefenderSpawn += OnDefenderSpawn;
            CoreGameEvents.Current.OnEnemySpawnerInitialize += OnSpawnerInitialize;
            CoreGameEvents.Current.onWin += OnWin;
            CoreGameEvents.Current.onLose += OnLose;
        }

        void OnDestroy() {
            CoreGameEvents.Current.OnCoreStarterFinished -= OnCoreStarterFinished;
            CoreGameEvents.Current.OnEnemySpawn -= OnEnemySpawn;
            CoreGameEvents.Current.OnEnemyReachEnd -= OnEnemyReachEnd;
            CoreGameEvents.Current.OnEnemyDestroy -= OnEnemyDestroy;
            CoreGameEvents.Current.OnDefenderDestroy -= OnDefenderDestroy;
            CoreGameEvents.Current.OnDefenderSpawn -= OnDefenderSpawn;
            CoreGameEvents.Current.OnEnemySpawnerInitialize -= OnSpawnerInitialize;
            CoreGameEvents.Current.onWin -= OnWin;
            CoreGameEvents.Current.onLose -= OnLose;
        }

        public void RestartGame() {
            BackgroundRunner.Current.StartCoroutine( coroutine( _starter ) );

            IEnumerator coroutine(CoreStarter coreStarter) {
                LoadingScreenManager.Current.StartLoadingScreen();
                yield return null;
                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                bool canContinue = false;
                BackgroundRunner.Current.StartCoroutine( coreStarter.StartGame( () => canContinue = true ) );
                yield return new WaitUntil( () => canContinue );
                LoadingScreenManager.Current.EndLoadingScreen();
            }
        }

        public void BackToLobby() {
            BackgroundRunner.Current.StartCoroutine( enumerator() );

            IEnumerator enumerator() {
                LoadingScreenManager.Current.StartLoadingScreen();
                yield return SceneManager.LoadSceneAsync( SceneDatabase.Instance.GetScenePath( "lobby" ), LoadSceneMode.Additive );
                yield return SceneManager.UnloadSceneAsync( gameObject.scene );
                LoadingScreenManager.Current.EndLoadingScreen();
            }
        }
        
        
        void OnCoreStarterFinished(CoreStarter coreStarter) {
            _starter = Instantiate( coreStarter );
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
                Debug.Log( $"Won Game!" );
                CoreGameEvents.Current.onWin?.Invoke();
            }
        }

        void OnEnemyReachEnd(Enemy enemy) {
            Destroy( enemy.gameObject );
            enemies.Remove( enemy );
            life -= 1;
            if (life <= 0 && !_lost) {
                _lost = true;
                Debug.Log( $"Lost Game!" );
                CoreGameEvents.Current.onLose?.Invoke();
            }
        }

        async void OnWin() {
            await Task.Delay( (int)(winDialogueDelay * 1000) );
            var dialogue =
                DialogueManager.Current.GetOrCreate<WinDialogue>( parentTransform: CoreUI.Current.transform );
            dialogue.stars = 3;

            // Tweener.Generate( () => Time.timeScale, v => Time.timeScale = v, 0, Ease.OutCubic, 1 );
            // dialogue.onClose += () => Time.timeScale = 1;
        }

        async void OnLose() {
            await Task.Delay( (int)(loseDialogueDelay * 1000) );
            var dialogue =
                DialogueManager.Current.GetOrCreate<LoseDialogue>( parentTransform: CoreUI.Current.transform );
            dialogue.onLobbyClick += BackToLobby;
            
            // Tweener.Generate( () => Time.timeScale, v => Time.timeScale = v, 0, Ease.OutCubic, 1 );
            // dialogue.onClose += () => Time.timeScale = 1;
        }
    }
}