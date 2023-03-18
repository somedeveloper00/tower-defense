using System;
using System.Collections;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.Defenders {
    [DeclareFoldoutGroup("anim", Title = "Animations")]
    public class MonkeyDefender : Defender {
        [SerializeField] Animator animator;

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

        void updateRotation() {
            if ( focusedenemy == null ) return;
            var target = focusedenemy.transform.position - transform.position;
            target.y = 0;
            transform.forward = target;
        }

        protected override IEnumerator Attack(Action applyAttack) {
            animator.SetFloat( animAttackSpeedName, animAttackSpeedValue );
            animator.CrossFade( animAttack, animCrossfade );
            yield return new WaitForSeconds( animAttackDamageTime );
            applyAttack();
        }
    }
}