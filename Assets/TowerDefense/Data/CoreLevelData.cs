using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using TowerDefense.Transport;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Data {
    [CreateAssetMenu( fileName = "Level", menuName = "Core/Level", order = 0 )]
    public class CoreLevelData : ScriptableObject, ITransportable {
        [Dropdown( nameof(_all_envs) )] public string env;
        public string title;
        public int id;
        public List<string> arguments = new();
        public List<EnemySpawner.SpawningMethod> spawnings = new();

        /// <summary>
        /// time limit for each star
        /// </summary>
        public int[] starTime = new int[3];

        public float coinMultiplier = 1;

        /// <summary>
        /// the bonus coin per star
        /// </summary>
        public ulong[] coinBonusForStars;

        void OnValidate() {
            if (starTime.Length != 3) Array.Resize( ref starTime, 3 );
            if (coinBonusForStars.Length != 3) Array.Resize( ref coinBonusForStars, 3 );
        }


        string[] _all_envs => SceneDatabase.Instance.GetAllNames();

        public string ToJson() => JsonConvert.SerializeObject( new {
            title, id, env, arguments, spawnings
        } );

        public void FromJson(string json) {
            arguments.Clear();
            spawnings.Clear();
            JsonConvert.PopulateObject( json, this );
        }

#region Helpers

        public int EvaluateStar(float time) {
            for (int i = starTime.Length - 1; i >= 0; i--)
                if (time < starTime[i]) return i + 1;
            return 0;
        }

        public ulong EvaluateBonusCoinForStar(int star) {
            ulong r = 0;
            for (int i = 0; i < star; i++) r += coinBonusForStars[i];
            return r;
        }

#endregion
    }
}