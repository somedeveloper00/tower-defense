﻿// On the basis: https://forum.unity.com/threads/export-obj-while-runtime.252262/
// How to use:
// Put file to Assets/Editor folder
// then select GameObject -> Export selected objects
// in Unity's main panel.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TowerDefense {

    struct ObjMaterial {
        public string name;
        public string textureName;
    }

    public class EditorObjExporter : ScriptableObject {
        private static int vertexOffset;
        private static int normalOffset;
        private static int uvOffset;

        // Output folder.
        private static string targetFolder = "_ExportedObj";

        private static string MeshToString(MeshFilter mf, Dictionary<string, ObjMaterial> materialList) {
            Mesh m = mf.sharedMesh;
            Material[] mats = mf.GetComponent<Renderer>().sharedMaterials;

            StringBuilder sb = new StringBuilder();

            string groupName = mf.name;
            if (string.IsNullOrEmpty( groupName )) {
                groupName = getRandomStr();
            }

            sb.Append( "g " ).Append( groupName ).Append( "\n" );
            foreach (Vector3 lv in m.vertices) {
                Vector3 wv = mf.transform.TransformPoint( lv );

                // This is sort of ugly - inverting x-component since we're in
                // a different coordinate system than "everyone" is "used to".
                sb.Append( string.Format(
                    "v {0} {1} {2}\n",
                    floatToStr( -wv.x ),
                    floatToStr( wv.y ),
                    floatToStr( wv.z )
                ) );
            }

            sb.Append( "\n" );

            foreach (Vector3 lv in m.normals) {
                Vector3 wv = mf.transform.TransformDirection( lv );

                sb.Append( string.Format(
                    "vn {0} {1} {2}\n",
                    floatToStr( -wv.x ),
                    floatToStr( wv.y ),
                    floatToStr( wv.z )
                ) );
            }

            sb.Append( "\n" );

            foreach (Vector3 v in m.uv) {
                sb.Append( string.Format(
                    "vt {0} {1}\n",
                    floatToStr( v.x ),
                    floatToStr( v.y )
                ) );
            }

            for (int material = 0; material < m.subMeshCount; material++) {
                sb.Append( "\n" );
                sb.Append( "usemtl " ).Append( mats[material].name ).Append( "\n" );
                sb.Append( "usemap " ).Append( mats[material].name ).Append( "\n" );

                // See if this material is already in the material list.
                try {
                    ObjMaterial objMaterial = new ObjMaterial { name = mats[material].name };

                    if (mats[material].mainTexture)
                        objMaterial.textureName = EditorUtility.GetAssetPath( mats[material].mainTexture );
                    else
                        objMaterial.textureName = null;

                    materialList.Add( objMaterial.name, objMaterial );
                }
                catch (ArgumentException) {
                    // Already in the dictionary
                }


                int[] triangles = m.GetTriangles( material );
                for (int i = 0; i < triangles.Length; i += 3) {
                    // Because we inverted the x-component, we also needed to alter the triangle winding.
                    sb.Append(
                        string.Format(
                            "f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                            triangles[i + 0] + 1 + vertexOffset,
                            triangles[i + 1] + 1 + normalOffset,
                            triangles[i + 2] + 1 + uvOffset
                        )
                    );
                }
            }

            vertexOffset += m.vertices.Length;
            normalOffset += m.normals.Length;
            uvOffset += m.uv.Length;

            return sb.ToString();
        }

        private static void Clear() {
            vertexOffset = 0;
            normalOffset = 0;
            uvOffset = 0;
        }

        private static Dictionary<string, ObjMaterial> PrepareFileWrite() {
            Clear();

            return new Dictionary<string, ObjMaterial>();
        }


        // https://stackoverflow.com/a/13680458/17089583
        private static void MeshToFile(MeshFilter mf, string folder, string filename) {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter sw = new StreamWriter( folder + "/" + filename + ".obj" )) {
                sw.Write( "mtllib ./" + filename + ".mtl\n" );

                sw.Write( MeshToString( mf, materialList ) );
            }
        }

        private static void MeshesToFile(MeshFilter[] mf, string folder, string filename) {
            Dictionary<string, ObjMaterial> materialList = PrepareFileWrite();

            using (StreamWriter sw = new StreamWriter( folder + "/" + filename + ".obj" )) {
                sw.Write( "mtllib ./" + filename + ".mtl\n" );

                for (int i = 0; i < mf.Length; i++) {
                    sw.Write( MeshToString( mf[i], materialList ) );
                }
            }
        }

        private static bool CreateTargetFolder() {
            try {
                Directory.CreateDirectory( targetFolder );
            }
            catch {
                return false;
            }

            return true;
        }

        [MenuItem( "GameObject/Export selected objects" )]
        static void ExportWholeSelectionToSingle() {
            try {
                
                EditorUtility.DisplayProgressBar( "OBJ file export", "initializing...", 0 );

                if (!CreateTargetFolder()) {
                    EditorUtility.DisplayDialog(
                        "Folder creating error!",
                        "Failed to create target folder " + targetFolder,
                        "OK"
                    );
                    return;
                }

                Transform[] selection = Selection.GetTransforms( SelectionMode.Editable | SelectionMode.ExcludePrefab );

                if (selection.Length == 0) {
                    EditorUtility.DisplayDialog(
                        "No source object selected!",
                        "Please select one or more target objects",
                        "OK"
                    );
                    return;
                }

                int exportedObjects = 0;

                ArrayList mfList = new ArrayList();

                EditorUtility.DisplayProgressBar( "OBJ file export", "getting meshes", 0.1f );
                
                for (int i = 0; i < selection.Length; i++) {
                    Component[] meshfilter = selection[i].GetComponentsInChildren( typeof(MeshFilter) );

                    for (int m = 0; m < meshfilter.Length; m++) {
                        exportedObjects++;
                        mfList.Add( meshfilter[m] );
                    }
                }

                
                if (exportedObjects > 0) {
                    
                    EditorUtility.DisplayProgressBar( "OBJ file export", "making file ready...", 0.2f );
                    
                    MeshFilter[] mf = new MeshFilter[mfList.Count];

                    for (int i = 0; i < mfList.Count; i++) {
                        mf[i] = (MeshFilter)mfList[i];
                    }

                    string filename = EditorApplication.currentScene + "-" + exportedObjects;

                    int stripIndex = filename.LastIndexOf( '/' ); //FIXME: Path.PathSeparator here (?)

                    if (stripIndex >= 0)
                        filename = filename.Substring( stripIndex + 1 ).Trim();

                    EditorUtility.DisplayProgressBar( "OBJ file export", "creating file...", 0.2f );
                    
                    MeshesToFile( mf, targetFolder, filename );

                    EditorUtility.ClearProgressBar();
                    
                    bool prompt = EditorUtility.DisplayDialog(
                        "Objects exported",
                        "Exported " + exportedObjects + " objects to " + filename,
                        "open in explorer",
                        "OK, thanks!"
                    );
                    if (prompt) {
                        if (!ExploreFile( targetFolder + "/" + filename + ".obj" )) {
                            Debug.Log( $"could not open :(" );
                        }
                    }
                }
                else {
                    EditorUtility.DisplayDialog(
                        "Objects not exported",
                        "Make sure at least some of your selected objects have mesh filters!",
                        "OK"
                    );
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }
        
        private static bool ExploreFile(string filePath) {
            if (!File.Exists(filePath)) {
                return false;
            }
            //Clean up file path so it can be navigated OK
            filePath = Path.GetFullPath(filePath);
            Process.Start("explorer.exe", $"/select,\"{filePath}\"" );
            return true;
        }

        private static string floatToStr(float number) {
            return String.Format( "{0:0.######}", number );
        }

        private static string getRandomStr() {
            string s = Path.GetRandomFileName() + DateTime.Now.Millisecond;
            s = s.Replace( ".", "" );

            return s;
        }
    }
}