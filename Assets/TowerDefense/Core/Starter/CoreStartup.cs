using System;
using System.Collections;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TowerDefense.Core.Starter {
    public static class CoreStartup {
        
        /// <summary>
        /// creates a copy of the level and loads the core and executes all the necessary calls to startup the gameplay
        /// </summary>
        /// <returns></returns>
        public static IEnumerator StartCore(CoreLevelData level, Action onComplete = null) {
            var clevel = ScriptableObject.CreateInstance<CoreLevelData>();
            clevel.TakeFrom( level );
            var path = SceneDatabase.Instance.GetScenePath( clevel.env );
            yield return null;
            yield return SceneManager.LoadSceneAsync( path, LoadSceneMode.Additive );
            SceneManager.SetActiveScene( SceneManager.GetSceneByPath( path ) );
            var spawners = Object.FindObjectsOfType<EnemySpawner>();
            for (var i = 0; i < spawners.Length; i++) {
                spawners[i].spawningMethod = clevel.spawnings[i];
                CoreGameEvents.Current.OnEnemySpawnerInitialize?.Invoke( spawners[i] );
            }

            onComplete?.Invoke();
            CoreGameEvents.Current.OnStartupFinished?.Invoke( clevel );
        }
    }
}