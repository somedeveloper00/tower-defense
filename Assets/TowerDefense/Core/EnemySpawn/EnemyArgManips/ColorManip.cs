using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Core.Enemies;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.EnemySpawn {
    [DisplayName("col")]
    public class ColorManip : EnemyArgManip {

        [JsonIgnore]
        [ShowInInspector]
        public List<Color> colors {
            get => cols.Select( sc => {
                if (ColorUtility.TryParseHtmlString( "#"+sc, out var col )) return col;
                return Color.white;
            } ).ToList();
            set => cols = value.Select( ColorUtility.ToHtmlStringRGBA ).ToList();
        }

        [JsonProperty] List<string> cols = new List<string>();
        
        protected override void Apply(Enemy enemy) {
            if (colors is null || colors.Count == 0) return;
            var rends = enemy.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rends.Length; i++) {
                rends[i].material.color = colors[i >= colors.Count ? colors.Count - 1 : i];
            }
        }
    }
}