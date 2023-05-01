using System;
using AnimFlex.Core.Proxy;
using AnimFlex.Sequencer.Clips;
using AnimFlex.Tweening;
using TriInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TowerDefense.Shaders {
    [ExecuteAlways]
    public class OutlineMesh : MonoBehaviour {
        [SerializeField] bool useSkinnedMeshSource;
        
        [ShowIf(nameof(useSkinnedMeshSource), true)]
        [SerializeField] SkinnedMeshRenderer sourceSkinnedMeshRenderer;
        
        [ShowIf(nameof(useSkinnedMeshSource), false)]
        [SerializeField] MeshFilter sourceMeshFilter;
        
        [SerializeField] MeshFilter meshFilter;
        public float length;

        void OnValidate() {
            meshFilter = GetComponent<MeshFilter>();
        }

        [Button]
        void forceUpdate() => meshFilter.mesh = null;

        void Update() {
            if (!meshFilter) return;
            
            Mesh sharedMesh = meshFilter.sharedMesh;

            if (useSkinnedMeshSource) {
                if (!sourceSkinnedMeshRenderer) return;
                if (!sharedMesh) {
                    sharedMesh ??= new Mesh();
                    meshFilter.sharedMesh = sharedMesh;
                }
                sourceSkinnedMeshRenderer.BakeMesh( meshFilter.sharedMesh, true );
                InflateMesh( sharedMesh );
            }
            else {
                if (!sourceMeshFilter) return;
                if (!sharedMesh) {
                    sharedMesh = Instantiate( sourceMeshFilter.sharedMesh );
                    meshFilter.sharedMesh = sharedMesh;
                    InflateMesh( sharedMesh );
                }
            }

        }

        void InflateMesh(Mesh sharedMesh) {
            var verts = sharedMesh.vertices;
            var normals = sharedMesh.normals;
            for (int i = 0; i < verts.Length; i++) {
                // move along normal direction of mesh
                verts[i] += normals[i] * length; // problem here!
            }

            sharedMesh.vertices = verts;
        }

        // animflex tweener clip
        [Serializable]
        public class OutlineMeshLengthClip : CTweener<TweenerGeneratorOutlineMeshLength> {
#if UNITY_EDITOR // for previewing in editor
            public override bool hasTick() => true;
            public override void Tick(float deltaTime) {
                Debug.Log( "tick" );
                if (!Application.isPlaying) {
                    tweenerGenerator.fromObject.Update();
                    SceneView.RepaintAll();
                }
            }
#endif
        }

        [Serializable]
        public class TweenerGeneratorOutlineMeshLength : TweenerGenerator<OutlineMesh, float> {
            protected override Tweener GenerateTween(AnimflexCoreProxy proxy) {
                return Tweener.Generate(
                    () => fromObject.length,
                    (val) => fromObject.length = val, target, duration, delay, ease, customCurve, () => fromObject );
            }
        }
    }
}