using System;
using Newtonsoft.Json.Linq;
using TriInspector;
using UnityEngine;
using TowerDefense.Core.Road;

namespace TowerDefense.Core.Enemies {
    public abstract class Enemy : MonoBehaviour {
        public float startingLife;
        public float life;
        public RoadManager roadManager;
        public float speed;
        public float rotationSpeed = 1;

        public event Action<Damage> onTakeDamage;
        public event Action<float> onLifeChanged;

        [ShowInInspector, ReadOnly] public RoadPiece currentRoadPiece { get; private set; }

        void Start() {
            if ( roadManager == null ) roadManager = FindObjectOfType<RoadManager>();
            life = startingLife;
            CoreGameEvents.Current.OnEnemySpawn?.Invoke( this );
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
            CoreGameEvents.Current.OnEnemyDestroy?.Invoke( this );
            Destroy( gameObject );
        }

        /// <summary>
        /// between 0 and 1
        /// </summary>
        public float GetRoadCompletion() {
            if ( currentRoadPiece == null ) return 0;
            if ( currentRoadPiece.next == null ) return 1;
            float w = 1f / (roadManager.roadPieces.Count - 1);
            float r = w * roadManager.IndexOfRoad( currentRoadPiece );
            // find progression from current road to next
            var vec1 = currentRoadPiece.next.Position - currentRoadPiece.Position;
            var vec2 = transform.position - currentRoadPiece.Position;
            r += Vector3.Dot( vec1, vec2 ) / vec1.magnitude / vec1.magnitude * w;
            return r;
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
                        CoreGameEvents.Current.OnEnemyReachEnd?.Invoke( this );
                        return;
                    }
                }

                transform.position += vec;
                transform.forward = Vector3.MoveTowards( transform.forward, vec.normalized, rotationSpeed * Time.deltaTime );
            }
        }
    }
}