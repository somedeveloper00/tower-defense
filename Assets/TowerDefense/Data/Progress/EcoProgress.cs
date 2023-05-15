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
        [SerializeField] long _coins = 40;

        [JsonIgnore]
        public long Coins => _coins;

        /// <summary>
        /// could be positive or negative
        /// </summary>
        public void AddToCoin(GameAnalyticsHelper.ItemType itemType, string details, long value) {
            try {
                GameAnalytics.NewResourceEvent( value > 0 ? GAResourceFlowType.Source : GAResourceFlowType.Sink,
                    "coin", value, itemType.ToAnalyticString(), details );
            } catch  { }

            _coins += value;
            if (_coins < 0) _coins = 0;
            Debug.Log( $"Coin increased by {value}, type: {itemType}" );
            onCoinsChanged?.Invoke( _coins );
        }

        public delegate void CoinChanged(long coinBefore);
        public event CoinChanged onCoinsChanged;
        
        public string ToJson() => JsonConvert.SerializeObject( this );
        public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
    }
}