using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Core.UI.Win {
    public class WinData : ITransportable {
        public int stars;
        public float time;
        public long coins;

        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}