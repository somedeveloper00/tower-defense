using System;
using System.Linq;
using TowerDefense.Core.Enemies;
using UnityEngine;

namespace TowerDefense.Core.Spawn {
    /// <summary>
    /// applies to enemies for manipulating their values
    /// </summary>
    [Serializable]
    public abstract class EnemyArgManip {
        [SerializeField, HideInInspector] string type;
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