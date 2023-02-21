using System;
using Core.Road;
using TriInspector;
using UnityEngine;

namespace Core
{
    public class Enemy : MonoBehaviour
    {
        public RoadManager roadManager;
        public float speed;

        public Action onFinishReached;

        [ShowInInspector, ReadOnly] RoadPiece currentRoadPiece;

        void Update() {
            if ( currentRoadPiece == null ) {
                currentRoadPiece = roadManager.GetNearestRoadPiece( transform.position );
            }

            move( speed * Time.deltaTime );
        }

        void move(float value) {
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
                        onFinishReached?.Invoke();
                    }
                }

                transform.position += vec;
                transform.forward = vec;
            }
        }
    }
}