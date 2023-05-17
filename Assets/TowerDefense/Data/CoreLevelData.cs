using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Core.EnemySpawn;
using TowerDefense.Core.Env;
using TowerDefense.Transport;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Data {
    [CreateAssetMenu( fileName = "Level", menuName = "TD/Core/Level", order = 0 )]
    public class CoreLevelData : ScriptableObject, ITransportable {
        [InfoBox("$info")]
        [JsonConverter(typeof(ColorConverter))]
        public Color btnCol;
        [Dropdown( nameof(_all_envs) )] 
        public string env;
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
        public long[] coinBonusForStars;

#if true
        string info() {
            var s = spawnings.Sum( sp => sp.onTime.Sum( t => t.spawns.Sum( s => s.count * EnemyDatabase.Current.GetEnemyWorth( s.name ) ) ) );
            return $"total coins: {s + coinBonusForStars[2]}, {s + coinBonusForStars[1]}, {s + coinBonusForStars[0]}";
        }
#endif
        

        void OnValidate() {
            if (lifeRemainForStar.Length != 3) Array.Resize( ref lifeRemainForStar, 3 );
            if (coinBonusForStars.Length != 3) Array.Resize( ref coinBonusForStars, 3 );
        }


        string[] _all_envs => SceneDatabase.Instance.GetAllNames();

        public string ToJson() => JsonConvert.SerializeObject( new {
            title, id, env, arguments, spawnings,
            btnCol = "#" + ColorUtility.ToHtmlStringRGB( btnCol ),
        } );

        public void FromJson(string json) {
            arguments.Clear();
            spawnings.Clear();
            JsonConvert.PopulateObject( json, this );
        }

#region Helpers

        public int EvaluateStar(float time, int startingLife, int endingLife) {
            var t = (float)endingLife / startingLife;
            for (int i = lifeRemainForStar.Length - 1; i >= 0; i--)
                if (t > lifeRemainForStar[i]) return i + 1;
            return 0;
        }

        public long EvaluateBonusCoinForStar(int star) => star > 0 ? coinBonusForStars[star - 1] : 0;

#endregion
    }
}