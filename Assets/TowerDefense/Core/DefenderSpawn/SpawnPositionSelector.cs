using System;
using TowerDefense.Input;
using UnityEngine;

namespace TowerDefense.Core.DefenderSpawn {
    public class SpawnPositionSelector : MonoBehaviour {
        public LayerMask raycastMask;
        public string illegalTag;
        [SerializeField] Transform placeholderTrans;
        [SerializeField] bool destroyPlaceholdersAfterSelect = true;
        [SerializeField] MaterialModify.Group legalMatGroup, illegalMatGroup;
        
        
        
        Camera _cam;
        bool _isOnLegalground = false;
        

        public event Action<Vector3> OnSelect;

        public void SetPlaceholder(GameObject prefab) {
            Instantiate( prefab, placeholderTrans );
        }
        
        void Start() => _cam = Camera.main;

        void Update() {
            var inp_pos = PointerDevice.Current.GetPointerPos();
            var ray = _cam.ScreenPointToRay( inp_pos );
            if (Physics.Raycast( ray, out var hit, 200, raycastMask )) {
                transform.position = hit.point;
                var isLegal = isLegalGround( hit.transform );
                if (_isOnLegalground != isLegal) {
                    _isOnLegalground = isLegal;
                    // apply material modification
                    foreach (var matmod in placeholderTrans.GetComponentsInChildren<MaterialModify>()) {
                        matmod.ApplyModIfMatch( isLegal ? legalMatGroup : illegalMatGroup );
                    }
                }
            }

            if (PointerDevice.Current.GetPointerUp() && _isOnLegalground) {
                Debug.Log( $"selected {transform.position}" );
                if (destroyPlaceholdersAfterSelect) {
                    foreach (Transform trans in placeholderTrans) {
                        Destroy( trans.gameObject );
                    }
                }
                OnSelect?.Invoke( transform.position );
            }
        }

        void setRendererColors(GameObject root, Color color) {
            var renderers = root.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
                if (renderers[i].sharedMaterial != null)
                    renderers[i].sharedMaterial.color = color;
        }
        
        bool isLegalGround(Transform transform) => !transform.CompareTag( illegalTag );
    }
}