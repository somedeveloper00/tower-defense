using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Core.Enemies;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.EnemySpawn {
    [DisplayName("col")]
    [Serializable]
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

        [JsonProperty, SerializeField, HideInInspector] List<string> cols = new List<string>();

        protected override void Apply(Enemy enemy) {
            if (colors is null || colors.Count == 0) return;
            var mats = enemy.GetComponentsInChildren<Renderer>().SelectMany( rend => rend.materials ).ToList();
            for (int i = 0; i < mats.Count; i++) {
                if (!mats[i]) continue;
                mats[i].color = colors[i >= colors.Count ? colors.Count - 1 : i];
            }
        }
    }
}