using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using TowerDefense.Transport;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Data {
    [CreateAssetMenu( fileName = "Level", menuName = "TD/Core/Level", order = 0 )]
    public class CoreLevelData : ScriptableObject, ITransportable {
        [Dropdown( nameof(_all_envs) )] public string env;
        public string title;
        public string id;
        public List<string> arguments = new();
        public List<EnemySpawner.SpawningMethod> spawnings = new();

        /// <summary>
        /// life percentage for each star
        /// </summary>
        [Range(0, 1)]
        public float[] lifeRemainForStar = new float[3];

        public float coinMultiplier = 1;

        /// <summary>
        /// the bonus coin per star
        /// </summary>
        public ulong[] coinBonusForStars;

        [Tooltip("The recommended amount of coins for this level")]
        public uint recomendedCoins = 100;
        

        void OnValidate() {
            if (lifeRemainForStar.Length != 3) Array.Resize( ref lifeRemainForStar, 3 );
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

        public int EvaluateStar(float time, int startingLife, int endingLife) {
            var t = (float)startingLife / endingLife;
            for (int i = lifeRemainForStar.Length - 1; i >= 0; i--)
                if (t > lifeRemainForStar[i]) return i + 1;
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