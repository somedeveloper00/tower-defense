using System;
using Core.Road;
using TriInspector;
using UnityEngine;

namespace Core.Enemies {
    public abstract class Enemy : MonoBehaviour {
        public float startingLife;
        public float life;
        public RoadManager roadManager;
        public float speed;

        public CoreGameEvents coreGameEvents;

        public event Action<Damage> onTakeDamage;
        public event Action<float> onLifeChanged;

        [ShowInInspector, ReadOnly] RoadPiece currentRoadPiece;

        void Start() {
            life = startingLife;
            coreGameEvents.OnEnemySpawn?.Invoke( this );
        }

        void Update() {
            moveForward( speed * Time.deltaTime );
        }

        public virtual void TakeDamage(Damage damage) {
            life -= damage.amount;
            if ( life < 0 ) life = 0;
            onTakeDamage?.Invoke( damage );
            onLifeChanged?.Invoke( life );
            if ( life == 0 ) destroy();
        }

        protected virtual void destroy() {
            coreGameEvents.OnEnemyDestroy?.Invoke( this );
            Destroy( gameObject );
        }

        void moveForward(float value) {
            if ( currentRoadPiece == null ) currentRoadPiece = roadManager.GetNearestRoadPiece( transform.position );
            if ( currentRoadPiece.next == null ) return;
            while (value > 0) {
                var vec = currentRoadPiece.next.Position - transform.position;
                if ( vec.magnitude > value ) {
                    vec = vec.normalized * value;
                    value = 0;
                }
                else {
                    value -= vec.magnitude;
                    currentRoadPiece = currentRoadPiece.next;
                    if ( currentRoadPiece.next == null ) {
                        coreGameEvents.OnEnemyReachEnd?.Invoke( this );
                        return;
                    }
                }

                transform.position += vec;
                transform.forward = vec;
            }
        }
    }
}