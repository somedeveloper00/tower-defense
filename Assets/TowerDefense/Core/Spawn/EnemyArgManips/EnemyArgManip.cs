using System;
using System.Linq;
using Newtonsoft.Json;
using TowerDefense.Core.Enemies;
using UnityEngine;

namespace TowerDefense.Core.Spawn {
    /// <summary>
    /// applies to enemies for manipulating their values
    /// </summary>
    [Serializable]
    [JsonConverter( typeof(SubTypeJsonConverter<EnemyArgManip>) )]
    public abstract class EnemyArgManip {
        [SerializeField, HideInInspector, JsonProperty( Order = -100000 )]
        string type;
        
        protected virtual Type[] appliableTypes() => new[] { typeof(Enemy) };

        protected abstract void Apply(Enemy enemy);
        
        bool CanApplyToEnemy(Enemy enemy) => appliableTypes().Contains( enemy.GetType() );

        public bool TryApplyToEnemy(Enemy enemy) {
            if ( CanApplyToEnemy( enemy ) ) {
                Apply( enemy );
                return true;
            }

            return false;
        }
    }
}