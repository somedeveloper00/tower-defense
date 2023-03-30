using System;
using System.Collections;
using System.Collections.Generic;
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

            string[] _all_envs => EnvDatabase.Instance.GetAllNames();

            public EnemySpawner.SpawningMethod[] spawnings;
        }

        public Data data;

        [Button]
        public IEnumerator StartGame() {
            var path = EnvDatabase.Instance.GetScenePath( data.env );
            var loadSceneAsync = SceneManager.LoadSceneAsync( path, LoadSceneMode.Additive );
            yield return loadSceneAsync;
            SceneManager.SetActiveScene( SceneManager.GetSceneByPath( path ) );
            var spawners = FindObjectsOfType<EnemySpawner>();
            for (int i = 0; i < spawners.Length; i++) {
                spawners[i].spawningMethod = data.spawnings[i];
                Debug.Log( $"spawner {spawners[i]} set up" );
            }
        }

        [Button]
        void _logJson() => Debug.Log( ToJson() );

        public string ToJson() => JsonConvert.SerializeObject( data );
        public void FromJson(string json) => data = JsonConvert.DeserializeObject<Data>( json );
        public void ApplyTo(CoreStarter target) => target.FromJson( ToJson() );
        public void TakeFrom(CoreStarter source) => FromJson( source.ToJson() );
    }
}