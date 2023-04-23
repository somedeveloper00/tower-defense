using System;
using TowerDefense.Input;
using UnityEngine;

namespace TowerDefense.Core.DefenderSpawn {
    public class SpawnPositionSelector : MonoBehaviour {
        public PointerAreaEvents clickAreaEvents;
        public LayerMask raycastMask;
        public string illegalTag;
        [SerializeField] Transform placeholderTrans;
        [SerializeField] MaterialModify.Group legalMatGroup, illegalMatGroup;
        
        Camera _cam;


        public bool isOnLegalGround { get; private set; }
        public event Action<Vector3> OnSettle;
        public event Action OnStartDrag;
        public event Action<Vector3> OnDrag;
        
        public void SetPlaceholder(GameObject prefab) {
            Instantiate( prefab, placeholderTrans );
        }

        public void DestroyPlaceholders() {
            foreach (Transform go in placeholderTrans) {
                Destroy( go.gameObject );
            }
        }

        void Start() {
            clickAreaEvents.onPointerUp += () => {
                if (isOnLegalGround) OnSettle?.Invoke( transform.position );
            };
            clickAreaEvents.onPointerDown += () => {
                if (isOnLegalGround) OnStartDrag?.Invoke();
            };
            _cam = Camera.main;
        }

        void Update() {

            if (!clickAreaEvents.isDown) return;
            
            var inp_pos = PointerDevice.Current.GetPointerPos();
            var ray = _cam.ScreenPointToRay( inp_pos );
            if (Physics.Raycast( ray, out var hit, 200, raycastMask )) {
                transform.position = hit.point;
                var isLegal = isLegalGround( hit.transform );
                if (isOnLegalGround != isLegal) {
                    isOnLegalGround = isLegal;
                    // apply material modification
                    foreach (var matmod in placeholderTrans.GetComponentsInChildren<MaterialModify>()) {
                        matmod.ApplyModIfMatch( isLegal ? legalMatGroup : illegalMatGroup );
                    }
                }
            }


            OnDrag?.Invoke( transform.position );
        }

        bool isLegalGround(Transform transform) => !transform.CompareTag( illegalTag );
    }
}