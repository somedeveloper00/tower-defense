using TowerDefense.Transport;
using UnityEditor;
using UnityEngine;

namespace TowerDefense.Data.Database {
    public class PreferencesDatabase : Database {
        public static PreferencesDatabase Current;
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
        static void start() => Current = new PreferencesDatabase();

        PreferencesDatabase() { }

        public override void Load() { }
        public override void Save() => PlayerPrefs.Save();
        public override bool KeyExists(string key) => PlayerPrefs.HasKey( key );

        public override bool GetValue(string key, ITransportable result) {
            if (!PlayerPrefs.HasKey( key )) return false;
            result.FromJson( PlayerPrefs.GetString( key ) );
            return true;
        }

        public override void Set(string key, ITransportable obj) => PlayerPrefs.SetString( key, obj.ToJson() );
        public override void Delete(string key) => PlayerPrefs.DeleteKey( key );
        public override void DeleteAll() => PlayerPrefs.DeleteAll();
    }
}