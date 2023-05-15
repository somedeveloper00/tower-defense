using System;
using GameAnalyticsSDK;
using Newtonsoft.Json;
using TowerDefense.Bridges.Analytics;
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
        public ulong Coins => _coins;

        /// <summary>
        /// could be positive or negative
        /// </summary>
        public void AddToCoin(GameAnalyticsHelper.ItemType itemType, string details, ulong value) {
            var tmp = _coins;
            _coins = value;
            try {
                GameAnalytics.NewResourceEvent( _coins > tmp ? GAResourceFlowType.Source : GAResourceFlowType.Sink,
                    "coin", value - tmp, itemType.ToAnalyticString(), details );
            } catch  { }
            onCoinsChanged?.Invoke( tmp );
        }

        public delegate void CoinChanged(ulong coinBefore);
        public event CoinChanged onCoinsChanged;
        
        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}