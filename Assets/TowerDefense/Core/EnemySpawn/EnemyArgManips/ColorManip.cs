using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Core.Enemies;
using UnityEngine;

namespace TowerDefense.Core.EnemySpawn {
    [DisplayName("col")]
    public class ColorManip : EnemyArgManip {
        [JsonIgnore] public List<Color> colors = new();

        [JsonProperty]
        List<string> cols {
            get => colors.Select( ColorUtility.ToHtmlStringRGB ).ToList();
            set => colors = value.Select( v => {
                if ( ColorUtility.TryParseHtmlString( v, out var col ) ) return col;
                return Color.white;
            } ).ToList();
        }
        
        
        protected override void Apply(Enemy enemy) {
            var rends = enemy.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < rends.Length; i++) {
                if ( colors.Count >= i ) break;
                rends[i].material.color = colors[i];
            }
        }
    }
}