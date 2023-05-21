using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TowerDefense.Transport;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TowerDefense.Data.Database {
    public class SecureDatabase : Database {

        string path;
        public SecureDatabase(string filepath) {
            path = Path.Combine( Application.persistentDataPath, filepath );
            Current = this;
        }

#if UNITY_EDITOR
        [MenuItem("Files/Open Secure Database File")]
        static void openPathInExplorer() {
            Process.Start( Path.Combine( Application.persistentDataPath, "s.dat" ) );
        }
        [MenuItem("Files/Delete Secure Database File")]
        static void delete() {
            File.Delete( Path.Combine( Application.persistentDataPath, "s.dat" ) );
        }
        [MenuItem("Files/Delete Secure Database File", true)]
        static bool delete_validation() {
            return File.Exists( Path.Combine( Application.persistentDataPath, "s.dat" ) );
        }
#endif

        public static SecureDatabase Current;

        
        Dictionary<string,string> data = new ();


        public override void Load() {
            data = new ();
            if (File.Exists( path )) {
                var fileData = decryptAndGetData( path );
                JsonConvert.PopulateObject( fileData, data );
            }
            Debug.Log( $"secure data loaded: {path}" );
        }

        public override void Save() {
            string txt = string.Empty;
            if (data != null) {
                txt = JsonConvert.SerializeObject( data );
            }
            encryptAndSaveData( txt, path );
        }

        public override bool KeyExists(string key) => data.ContainsKey( key );

        public override bool GetValue(string key, ITransportable result) {
            if (data.TryGetValue(key, out var token)) {
                try {
                    result.FromJson( token );
                }
                catch (Exception e) {
                    Debug.LogException( e );
                    return false;
                }
                return true;
            }
            return false;
        }

        public override void Set(string key, ITransportable obj) => data[key] = obj.ToJson();

        public override void Delete(string key) => data.Remove( key );

        public override void DeleteAll() {
            File.Delete( path );
            data = new();
        }


        static void encryptAndSaveData(string txt, string path) {
            File.WriteAllText( path, txt );
            // using var fs = new FileStream( path, FileMode.OpenOrCreate );
            // using var bw = new BinaryWriter( fs );
            // bw.Write( txt );
        }

        static string decryptAndGetData(string path) {
            return File.ReadAllText( path );
            // using var fs = new FileStream( path, FileMode.Open );
            // using var br = new BinaryReader( fs );
            // return br.ReadString();
        }
    }
}