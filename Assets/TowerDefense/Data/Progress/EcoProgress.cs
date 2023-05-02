using System;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Data.Progress {
    [Serializable]
    public class EcoProgress : ITransportable {

        public ulong coins = 20;
        
        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}