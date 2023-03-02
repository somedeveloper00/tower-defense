using System;
using Core.Road;
using TriInspector;
using UnityEngine;

namespace Core
{
    [ExecuteAlways]
    public class RoadManager : MonoBehaviour
    {
        public RoadPiece[] roadPieces;
#if UNITY_EDITOR
        [Title("Editor only")]
        [SerializeField] bool autoGetChildPieces;
        [SerializeField] bool autoConnectPieces;
#endif

        void Update() {
#if UNITY_EDITOR
            if ( !Application.isPlaying ) {
                if ( autoGetChildPieces ) {
                    roadPieces = GetComponentsInChildren<RoadPiece>();
                }
                if ( autoConnectPieces ) {
                    for (int i = 0; i < roadPieces.Length; i++) {
                        roadPieces[i].previous = i > 0 ? roadPieces[i - 1] : null;
                        roadPieces[i].next = i < roadPieces.Length - 1 ? roadPieces[i + 1] : null;
                    }
                }
            }
#endif
        }

        public RoadPiece GetNearestRoadPiece(Vector3 position) {
            var leastDist = float.MaxValue;
            RoadPiece r = null;
            foreach (var piece in roadPieces) {
                var d = (position - piece.Position).sqrMagnitude;
                // threshold check
                if ( d < 0.5f ) return piece;
                if ( d < leastDist ) {
                    leastDist = d;
                    r = piece;
                }
            }

            return r;
        }
    }
}