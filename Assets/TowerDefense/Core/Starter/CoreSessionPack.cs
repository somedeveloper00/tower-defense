using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Core.Starter {
    /// <summary>
    /// data for a core session. like coins and defenders etc.
    /// </summary>
    [Serializable]
    public class CoreSessionPack : ITransportable {
        public int life = 20;
        public long coins;
        public List<string> defenders = new();

        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
        public string ToJson() => JsonConvert.SerializeObject( this );
    }
}