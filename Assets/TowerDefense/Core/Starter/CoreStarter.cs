using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Core.Env;
using TowerDefense.Core.Spawn;
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

            List<string> _all_envs {
                get {
                    var envs = EnvDatabase.Instance.GetAllNames();
                    envs.Add( env );
                    return envs;
                }
            }

            public EnemySpawner.SpawningMethod[] spawnings;
        }

        public Data data;

        [Button]
        public IEnumerator StartGame() {
            var scene = EnvDatabase.Instance.GetScene( data.env );
            var loadSceneAsync = SceneManager.LoadSceneAsync( scene.buildIndex );
            yield return loadSceneAsync;
            SceneManager.SetActiveScene( scene );
            var spawners = FindObjectsOfType<EnemySpawner>();
            for (int i = 0; i < spawners.Length; i++) {
                spawners[i].spawningMethod = data.spawnings[i];
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