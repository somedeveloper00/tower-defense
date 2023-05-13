using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Core.Starter;
using TowerDefense.Transport;
using TriInspector;

namespace TowerDefense.Data {
    [Serializable]
    public class LevelsData : ITransportable {
        public List<CoreLevelData> coreLevels = new();

        public string ToJson() => JsonConvert.SerializeObject(this);
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}