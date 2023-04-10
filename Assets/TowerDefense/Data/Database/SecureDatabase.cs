using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TowerDefense.Transport;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TowerDefense.Data.Database {
    public class SecureDatabase : IDatabase {

        string path;
        public SecureDatabase(string filepath) => path = Path.Combine( Application.persistentDataPath, filepath );

#if UNITY_EDITOR
        [MenuItem("Files/Open Secure Database File")]
        static void openPathInExplorer() {
            Process.Start( Current.path );
        }
#endif

        public static SecureDatabase Current;
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        static void start() {
            Current = new SecureDatabase("s.dat");
            Current.Load();
            Debug.Log( $"secure data loaded." );
        }

        
        JObject data;


        public void Load() {
            data = new JObject();
            if (File.Exists( path )) {
                var fileData = decryptAndGetData( path );
                try {
                    data = JsonConvert.DeserializeObject<JObject>( fileData );
                }
                catch (Exception e) {
                    Debug.LogException( e );
                }
            }
        }

        public void Save() {
            string txt = string.Empty;
            if (data != null) {
                txt = JsonConvert.SerializeObject( data );
            }
            encryptAndSaveData( txt, path );
        }

        public bool KeyExists(string key) => data.GetValue( key ) != null;

        public T GetValue<T>(string key) where T : ITransportable, new() {
            var field = data.GetValue( key );
            if (field != null) {
                return field.ToObject<T>();
            }

            throw new Exception( "key not found: " + key );
        }

        public bool TryGetValue<T>(string key, out T result) where T : ITransportable, new() {
            if (data.TryGetValue(key, out var token)) {
                try {
                    result = token.ToObject<T>();
                    return true;
                } catch { }
            }
            result = default;
            return false;
        }

        public void Set(string key, ITransportable obj) {
            data[key] = JsonConvert.DeserializeObject<JObject>( obj.ToJson() );
        }

        public void Delete(string key) => data.Remove( key );

        public void DeleteAll() {
            File.Delete( path );
            data = new JObject();
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