using System;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Core.UI.Lose {
    [Serializable]
    public class LoseData : ITransportable {
        public float time;
        public long coins;

        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}