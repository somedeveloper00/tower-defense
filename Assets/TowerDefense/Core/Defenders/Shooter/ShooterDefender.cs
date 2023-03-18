using System;
using System.Collections;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.Defenders {
    [DeclareFoldoutGroup("anim", Title = "Anim Params")]
    public class ShooterDefender : Defender
    {
        [SerializeField] Animator animator;
        [SerializeField] float rotationSpeed = 20;
        [SerializeField] Transform shootEffectSpawnPoint;
        [SerializeField] GameObject shootEffect;
        
        [GroupNext( "anim" )] 
        [SerializeField, Range(0, 1)] float animCrossfade = 0.1f;
        [SerializeField] string animIdle;
        [SerializeField] string animAttack;
        [SerializeField] float animAttackDamageTime;
        [SerializeField] string animAttackSpeedName;
        [SerializeField] float animAttackSpeedValue;

        void Update() {
            updateFocusedEnemy();
            updateRotation();
            updateShoot();
        }

        protected override IEnumerator Attack(Action applyAttack) {
            animator.SetFloat( animAttackSpeedName, animAttackSpeedValue );
            animator.CrossFade( animAttack, animCrossfade );
            yield return new WaitForSeconds( animAttackDamageTime / animAttackSpeedValue );
            Instantiate( shootEffect, shootEffectSpawnPoint );
            applyAttack();
        }

        void updateRotation() {
            if ( this.focusedenemy == null ) return;
            var vec = this.focusedenemy.transform.position - transform.position;
            vec.y = 0;
            transform.forward = Vector3.RotateTowards( transform.forward, vec, rotationSpeed * Time.deltaTime,
                rotationSpeed * Time.deltaTime );
        }
        
        
    }
}