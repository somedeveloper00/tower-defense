using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TowerDefense.Core.Starter;
using TowerDefense.Transport;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Data {
    [Serializable]
    public class GameLevelsData : ITransportable {
        public List<Level> levels = new();

        [Serializable]
        public class Level {
            [InlineEditor]
            [JsonIgnore] public CoreLevelData gameData;
            public RuntimeData runtimeData;


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

        [Serializable]
        public class RuntimeData {
            public LevelStatus status;
            public int stars = 0;
            public int playCount = 0;
        }
        
        [Flags]
        public enum LevelStatus {
            Unlocked = 1 << 1,
            Finished = 1 << 2,
        }


        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) {
            levels.Clear();
            JsonConvert.PopulateObject( json, this );
        }
    }
}