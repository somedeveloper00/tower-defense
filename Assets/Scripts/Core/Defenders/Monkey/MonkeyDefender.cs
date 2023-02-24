using System;
using System.Collections;
using TriInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Defenders.Monkey {
    [DeclareFoldoutGroup("anim", Title = "Animations")]
    public class MonkeyDefender : Defender {
        [SerializeField] Animator animator;
        
        [GroupNext("anim")]
        [SerializeField] string animAttack;
        [SerializeField] string animIdle;
        [SerializeField] float idleAttackTime;
        
        
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
            animator.Play( animAttack );
            yield return new WaitForSeconds( idleAttackTime );
            applyAttack();
        }
    }
}