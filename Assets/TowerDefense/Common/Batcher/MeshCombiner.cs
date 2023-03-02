using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TriInspector;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TowerDefense.Common.Batcher
{
    [DeclareFoldoutGroup("op", Title = "Options", Expanded = false)]
    public class MeshCombiner : MonoBehaviour
    {
        public bool combineOnStart = false;
        
        [Group( "op" ), PropertyOrder( 15 )] public List<LayerResolve> LayerResolves;
        [Group( "op" ), PropertyOrder( 15 )] public bool disableGameObjects = true;
        [Group( "op" ), PropertyOrder( 15 )] public string resultParentName = "Combined";
        [Group( "op" ), PropertyOrder( 15 )] public bool takeShadowCast = true;
        [Group( "op" ), PropertyOrder( 15 )] public bool takeShadowReceive = true;

        [ReadOnly, PropertyOrder( 30 )] public List<MeshRenderer> results = new();

        
        Dictionary<Material, List<MeshRenderer>> _subjects;
        int subjectsCount => _subjects?.Sum( s => s.Value.Count ) ?? 0;

        string GetInfoBoxMessage() => $"fetched: {subjectsCount} subjects";

        void Start() {
            if ( combineOnStart ) {
                FetchSubjects();
                Combine();
            }
        }

        [Button, PropertyOrder(10)]
        public void FetchSubjects() {
            foreach (var meshRenderer in results) {
                if (meshRenderer != null) SafeDestroy( meshRenderer.gameObject );
            }

            results.Clear();

            _subjects = new();
            foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>()) {
                foreach (var mat in meshRenderer.sharedMaterials) {
                    // find group
                    List<MeshRenderer> group;
                    if ( !_subjects.TryGetValue( mat, out group ) ) {
                        group = new List<MeshRenderer>();
                        _subjects.Add( mat, group );
                    }

                    // add to group
                    group.Add( meshRenderer );
                }
            }
#if UNITY_EDITOR
            OnValidate();
#endif
        }

#if UNITY_EDITOR
        void OnValidate() {
            if ( _subjects == null ) return;
            if ( LayerResolves == null ) LayerResolves = new();
            var materials = _subjects.Keys.ToArray();
            for (int i = 0; i < materials.Length; i++) {
                if ( LayerResolves.Count <= i ) LayerResolves.Add( new LayerResolve() );
                LayerResolves[i].material = materials[i];
                if ( string.IsNullOrEmpty( LayerResolves[i].layer ) ) {
                    LayerResolves[i].layer = LayerMask.LayerToName( _subjects[materials[i]][0].gameObject.layer );
                }
            }
        }
#endif

        [InfoBox("$GetInfoBoxMessage", TriMessageType.Info)]
        [DisableIf("subjectsCount", 0)]
        [Button, PropertyOrder(20)]
        public void Combine() {
            combineOnStart = false;
            var parent = new GameObject( resultParentName );
            parent.transform.SetParent( transform );
            // combine
            foreach (var (mat, group) in _subjects) {
                var go = new GameObject( $"CombinedMesh_{mat.name}" );
                go.transform.SetParent( parent.transform );
                var meshFilter = go.AddComponent<MeshFilter>();
                var meshRenderer = go.AddComponent<MeshRenderer>();
                if ( takeShadowCast ) meshRenderer.shadowCastingMode = group[0].shadowCastingMode;
                if ( takeShadowReceive ) meshRenderer.receiveShadows = group[0].receiveShadows;
                meshRenderer.sharedMaterial = mat;
                results.Add( meshRenderer );
                CombineInstance[] combine = new CombineInstance[group.Count];
                for (int i = 0; i < group.Count; i++) {
                    combine[i].mesh = group[i].GetComponent<MeshFilter>().sharedMesh;
                    combine[i].transform = group[i].transform.localToWorldMatrix;
                }
                meshFilter.sharedMesh = new Mesh();
                meshFilter.sharedMesh.CombineMeshes( combine, true );
                
                // disable old
                foreach (var mRenderer in group) {
                    if (disableGameObjects)
                        mRenderer.gameObject.SetActive( false );
                    else
                        mRenderer.enabled = false;
                }
            }
            
            // resolve layers
            foreach (var layerResolve in LayerResolves) {
                var layer = LayerMask.NameToLayer( layerResolve.layer );
                foreach (var meshRenderer in results) {
                    if (meshRenderer.sharedMaterial == layerResolve.material) {
                        meshRenderer.gameObject.layer = layer;
                    }
                }
            }
        }

        void SafeDestroy(Object obj) {
#if UNITY_EDITOR
            if ( !Application.isPlaying )
                DestroyImmediate( obj );
            else
#endif
                Destroy( obj );
        }


        [Serializable]
        [DeclareHorizontalGroup( "h" )]
        public class LayerResolve
        {
            [GroupNext( "h" )] 
            [LabelWidth( 50 )] public Material material;
            [LabelWidth( 40 )] public string layer;
        }
    }
}