using Core.Road;
using UnityEngine;

namespace Core
{
    public class RoadManager : MonoBehaviour
    {
        public RoadPiece[] roadPieces;

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