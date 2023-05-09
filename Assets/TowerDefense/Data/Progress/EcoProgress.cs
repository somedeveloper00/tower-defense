using System;
using Newtonsoft.Json;
using TowerDefense.Transport;
using UnityEngine;
using UnityEngine.Serialization;

namespace TowerDefense.Data.Progress {
    [Serializable]
    public class EcoProgress : ITransportable {

        [FormerlySerializedAs("coins")]
        [JsonProperty("coins")]
        [SerializeField] ulong _coins = 40;

        [JsonIgnore]
        public ulong coins {
            get => _coins;
            set {
                var tmp = _coins;
                _coins = value;
                onCoinsChanged?.Invoke( tmp );
            }
        }
        
        public delegate void CoinChanged(ulong coinBefore);
        public event CoinChanged onCoinsChanged;
        
        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}