using System;
using System.Linq;
using System.Collections;
using JetBrains.Annotations;
using TowerDefense.Core.Enemies;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.Core.Defenders {
    public abstract class Defender : MonoBehaviour { 
        public float attackRange;
        public float attackPower;
        public float attackReloadTime;
        public ulong cost;
        
        protected Enemy focusedenemy;
        protected float timeSinceLastAttack;

#if UNITY_EDITOR
        protected virtual void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere( transform.position, attackRange );
            Handles.color = new (Color.red.r, Color.red.g, Color.red.b, 0.25f);
            Handles.DrawSolidDisc( transform.position, Vector3.down, attackRange );
        }
#endif

        protected float getTimeToNextAttack() {
            if ( timeSinceLastAttack == 0 ) return 0;
            var t = Time.time - timeSinceLastAttack;
            return t > attackReloadTime ? 0 : attackReloadTime - t;
        }

        protected void updateFocusedEnemy() {
            bool shouldFindNewEnemy =
                focusedenemy == null ||
                Vector3.Distance( focusedenemy.transform.position, transform.position ) > attackRange;
            
            if ( shouldFindNewEnemy ) {
                var enemiesInRange = CoreGameManager.Current.enemies.Where( e =>
                    Vector3.Distance( e.transform.position, transform.position ) < attackRange );
                // get one that's closer to escaping
                Enemy enem = null;
                float t_best = float.MinValue;
                foreach (var enemy in enemiesInRange) {
                    var t = enemy.GetRoadCompletion();
                    if ( t > t_best ) {
                        enem = enemy;
                        t_best = t;
                    }
                }
                focusedenemy = enem;
            }
        }
        
        protected virtual Damage createNewDamage() {
            return new() { amount = attackPower };
        }

        protected virtual void updateShoot() {
            if ( getTimeToNextAttack() > 0 ) return;
            if ( focusedenemy == null ) return;
            timeSinceLastAttack = Time.time;
            StartCoroutine( Attack( () => {
                if ( focusedenemy == null ) return;
                var damage = createNewDamage();
                focusedenemy.TakeDamage( damage );
            } ) );
        }

        protected abstract IEnumerator Attack([NotNull] Action applyAttack);
    }
}