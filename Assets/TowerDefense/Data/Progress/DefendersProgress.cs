using System;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Data.Progress {
    [Serializable]
    public class DefendersProgress : ITransportable {
        public string[] defenders;
        
        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}