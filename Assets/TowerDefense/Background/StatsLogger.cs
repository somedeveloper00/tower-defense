using System;
using System.Collections.Generic;
using System.Linq;
using TriInspector;
using UnityEngine;

namespace TowerDefense.Background {
    public class StatsLogger : MonoBehaviour {
        void Start() => InvokeRepeating( nameof(report), 1, 1 );

        [Button]
        void report() {
            string log = String.Empty;
            var ms = (int)( Time.unscaledDeltaTime * 1000 );
            int meshCount;
            var sharedMeshes = new HashSet<Mesh>();
            var verteces = FindObjectsOfType<MeshFilter>().Sum( mesh => {
                if (mesh && sharedMeshes.Add( mesh.sharedMesh )) return mesh.sharedMesh.vertexCount;
                return 0;
            } );
            meshCount = sharedMeshes.Count;
            sharedMeshes.Clear();
            var vertecesSkinned = FindObjectsOfType<SkinnedMeshRenderer>().Sum( mesh => {
                if (mesh && sharedMeshes.Add( mesh.sharedMesh )) return mesh.sharedMesh.vertexCount;
                return 0;
            } );
            meshCount += sharedMeshes.Count;
            log = $"report: {ms}ms/{1000/ms:F0}fps - meshes:{meshCount} verts:{verteces:#,0} verts(skinned):{vertecesSkinned:#,0} verts(total):{verteces+vertecesSkinned:#,0}";
            Debug.Log( log );
        }
    }
}