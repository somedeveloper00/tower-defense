using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TowerDefense.Core.Starter;
using TowerDefense.Transport;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Player {
    [Serializable]
    public class GameLevelsData : ITransportable {
        public List<Level> levels = new();

        [Serializable]
        public class Level {
            public LevelStatus status;
            [InlineEditor]
            [JsonIgnore] public CoreLevelData gameData;

            [JsonProperty( "gameData" )]
            JObject gameDataJson {
                get {
                    if (gameData)
                        return JsonConvert.DeserializeObject<JObject>( gameData.ToJson() );
                    return new JObject();
                }
                set {
                    if (!gameData) gameData = ScriptableObject.CreateInstance<CoreLevelData>();
                    gameData.FromJson( value.ToString() );
                }
            }
        }

        [Flags]
        public enum LevelStatus {
            Unlocked = 1 << 1,
            Finished = 1 << 2
        }


        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) {
            levels.Clear();
            JsonConvert.PopulateObject( json, this );
        }
    }
}