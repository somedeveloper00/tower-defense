using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Core.Starter;
using TowerDefense.Transport;

namespace TowerDefense.Player {
    [Serializable]
    public class GameData : ITransportable {
        public List<Level> levels = new();

        [Serializable]
        public class Level {
            public string title;
            public List<string> arguments;
            public CoreStarter starter;
        }

        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) {
            levels.Clear();
            JsonConvert.PopulateObject( json, this );
        }
    }
}