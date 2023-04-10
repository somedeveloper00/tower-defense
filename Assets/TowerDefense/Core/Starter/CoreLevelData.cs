using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using TowerDefense.Transport;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.Starter {
    [CreateAssetMenu( fileName = "Level", menuName = "Core/Level", order = 0 )]
    public class CoreLevelData : ScriptableObject, ITransportable, IDataExchange<CoreLevelData> {
        [Dropdown( nameof(_all_envs) )] public string env;
        public string title;
        public int id;
        public List<string> arguments = new();
        public List<EnemySpawner.SpawningMethod> spawnings = new();
        public int[] starTime = new int[3];

        void OnValidate() {
            if (starTime.Length != 3)
                Array.Resize( ref starTime, 3 );
        }


        string[] _all_envs => SceneDatabase.Instance.GetAllNames();

        public void ApplyTo(CoreLevelData target) => target.FromJson( ToJson() );
        public void TakeFrom(CoreLevelData source) => FromJson( source.ToJson() );

        public string ToJson() => JsonConvert.SerializeObject( new {
            title, id, env, arguments, spawnings
        } );

        public void FromJson(string json) {
            arguments.Clear();
            spawnings.Clear();
            JsonConvert.PopulateObject( json, this );
        }
    }
}