using System;
using System.Collections;
using Newtonsoft.Json;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using TowerDefense.Transport;
using TriInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense.Core.Starter {
    [CreateAssetMenu( fileName = "CoreStarter", menuName = "Core/Core Starter", order = 0 )]
    public class CoreStarter : ScriptableObject, ITransportable, IDataExchange<CoreStarter> {

        [Serializable]
        public class Data {
            [Dropdown(nameof(_all_envs))]
            public string env;

            string[] _all_envs => SceneDatabase.Instance.GetAllNames();

            public EnemySpawner.SpawningMethod[] spawnings;
        }

        public Data data;

        public IEnumerator StartGame(Action onComplete = null) {
            var path = SceneDatabase.Instance.GetScenePath( data.env );
            yield return null;
            yield return SceneManager.LoadSceneAsync( path, LoadSceneMode.Additive );
            SceneManager.SetActiveScene( SceneManager.GetSceneByPath( path ) );
            var spawners = FindObjectsOfType<EnemySpawner>();
            for (int i = 0; i < spawners.Length; i++) {
                spawners[i].spawningMethod = data.spawnings[i];
                CoreGameEvents.Current.OnEnemySpawnerInitialize?.Invoke( spawners[i] );
            }
            onComplete?.Invoke();
            CoreGameEvents.Current.OnCoreStarterFinished?.Invoke( this );
        }

        public string ToJson() => JsonConvert.SerializeObject( data );
        public void FromJson(string json) => data = JsonConvert.DeserializeObject<Data>( json );
        public void ApplyTo(CoreStarter target) => target.FromJson( ToJson() );
        public void TakeFrom(CoreStarter source) => FromJson( source.ToJson() );
    }
}