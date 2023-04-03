using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Player {
    [Serializable]
    public class PlayerData : ITransportable {
        public string name;
        public List<string> defenders = new();
        public int level;

        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) {
            defenders.Clear();
            JsonConvert.PopulateObject( json, this );
        }
    }
}