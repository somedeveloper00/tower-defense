using System;
using TriInspector;
using UnityEngine;
using UnityEngine.Animations;

namespace TowerDefense.Common
{
    [ExecuteAlways]
    public class LookAtMainCam : MonoBehaviour
    {
        [SerializeField] LookAtConstraint lookAtConstraint;
        
        [Button]
        void Start() {
            lookAtConstraint.SetSource( 0, new ConstraintSource() {
                weight = 1,
                sourceTransform = Camera.main.transform
            } );
            lookAtConstraint.constraintActive = true;
            lookAtConstraint.locked = false;
            lookAtConstraint.rotationOffset = new Vector3( 0, 180, 0 );
            lookAtConstraint.locked = true;
            Debug.Log( "STRARTED @ "+name );
        }
    }
}