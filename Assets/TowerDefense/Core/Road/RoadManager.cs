using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Core.Road {
    [ExecuteAlways]
    public class RoadManager : MonoBehaviour
    {
        public List<RoadPiece> roadPieces;
#if UNITY_EDITOR
        [Title("Editor only")]
        [SerializeField] bool autoGetChildPieces;
        [SerializeField] bool autoConnectPieces;
#endif

        void Update() {
#if UNITY_EDITOR
            if ( !Application.isPlaying ) {
                if ( autoGetChildPieces ) {
                    roadPieces = GetComponentsInChildren<RoadPiece>().ToList();
                }
                if ( autoConnectPieces ) {
                    for (int i = 0; i < roadPieces.Count; i++) {
                        roadPieces[i].previous = i > 0 ? roadPieces[i - 1] : null;
                        roadPieces[i].next = i < roadPieces.Count - 1 ? roadPieces[i + 1] : null;
                    }
                }
            }
#endif
        }

        public int IndexOfRoad(RoadPiece roadPiece) => roadPieces.IndexOf( roadPiece );
        
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