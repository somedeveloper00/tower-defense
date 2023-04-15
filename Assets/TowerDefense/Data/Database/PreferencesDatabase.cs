using TowerDefense.Transport;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Data.Database {
    public class PreferencesDatabase : IDatabase {
        
        public static PreferencesDatabase Current;
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        static void start() => Current = new PreferencesDatabase();
        PreferencesDatabase() { }

        public void Load() { }
    public void Save() => PlayerPrefs.Save();
        public bool KeyExists(string key) => PlayerPrefs.HasKey( key );
        public T GetValue<T>(string key) where T : ITransportable, new() {
            var r = new T();
            r.FromJson( PlayerPrefs.GetString( key ) );
            return r;
        }

        public bool TryGetValue<T>(string key, out T result) where T : ITransportable, new() {
            if (PlayerPrefs.HasKey( key )) {
                result = GetValue<T>( key );
                return true;
            }

            result = default;
            return false;
        }

        public void Set(string key, ITransportable obj) => PlayerPrefs.SetString( key, obj.ToJson() );
        public void Delete(string key) => PlayerPrefs.DeleteKey( key );
        public void DeleteAll() => PlayerPrefs.DeleteAll();
    }
}