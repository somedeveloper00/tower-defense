using UnityEngine;

namespace Core.Road
{
    public class RoadPiece : MonoBehaviour
    {
        public RoadPiece previous, next;

        public Vector3 Position => transform.position;

        void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            if (previous != null)
                Gizmos.DrawLine( nicePos( transform ), nicePos( previous.transform ) );
            Gizmos.color = Color.red;
            if (next != null)
                Gizmos.DrawLine( nicePos( transform, 0.15f ), nicePos( next.transform, 0.15f ) );
            
            Vector3 nicePos(Transform trans, float d = 0.1f) => trans.position + trans.up * d;
        }
    }
}