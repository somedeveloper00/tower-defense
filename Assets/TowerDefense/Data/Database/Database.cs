using System;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Data.Database {
    public abstract class Database {
        public abstract void Load();
        public abstract void Save();
        public abstract bool KeyExists(string key);
        
        public abstract bool TryGetValue<T>(string key, out T result) where T : ITransportable, new();
        public abstract T GetValue<T>(string key) where T : ITransportable, new();
        public abstract void Set(string key, ITransportable obj);
        public abstract void Delete(string key);
        public abstract void DeleteAll();

        public bool TryGetInt(string key, out int result) {
            var r = TryGetValue( key, out IntTransport intTrans );
            result = intTrans.value;
            return r;
        }

        public bool TryGetFloat(string key, out float result) {
            var r = TryGetValue( key, out FloatTransport intTrans );
            result = intTrans.value;
            return r;
        }

        public bool TryGetString(string key, out string result) {
            var r = TryGetValue( key, out StringTransport intTrans );
            result = intTrans.value;
            return r;
        }
        

        public void Set(string key, int obj) => Set( key, new IntTransport() { value = obj } );
        public void Set(string key, float obj) => Set( key, new FloatTransport() { value = obj } );
        public void Set(string key, string obj) => Set( key, new StringTransport() { value = obj } );

        public int GetInt(string key) => GetValue<IntTransport>( key ).value;
        public float GetFloat(string key) => GetValue<FloatTransport>( key ).value;
        public string GetString(string key) => GetValue<StringTransport>( key ).value;
        


        [Serializable]
        abstract class BasicTransform<T> : ITransportable {
            public T value;
            public string ToJson() => JsonConvert.SerializeObject( this );
            public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
        }
        [Serializable] class IntTransport : BasicTransform<int> { }
        [Serializable] class FloatTransport : BasicTransform<float> { }
        [Serializable] class StringTransport : BasicTransform<string> { }
    }
}