using System;
using System.Collections;
using Core.Enemies;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Defenders {
    public abstract class Defender : MonoBehaviour {
        public GameManager gameManager;
        public float attackRange;
        public float attackPower;
        public float attackReloadTime;
        
        protected Enemy focusedenemy;
        protected float timeSinceLastAttack;

        protected virtual void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere( transform.position, attackRange );
        }

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
                var enemy = gameManager.enemies.Find( e =>
                    Vector3.Distance( e.transform.position, transform.position ) < attackRange );
                focusedenemy = enemy;
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
                var damage = createNewDamage();
                focusedenemy.TakeDamage( damage );
                Debug.Log( $"{name} Attacked {focusedenemy.name}: \n{JsonConvert.SerializeObject( damage )}" );
            } ) );
        }

        protected abstract IEnumerator Attack([NotNull] Action applyAttack);
    }
}