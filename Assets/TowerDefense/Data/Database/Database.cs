using System;
using Newtonsoft.Json;
using TowerDefense.Transport;

namespace TowerDefense.Data.Database {
    public abstract class Database {
        public abstract void Load();
        public abstract void Save();
        public abstract bool KeyExists(string key);

        public abstract bool GetValue(string key, ITransportable target);
        public abstract void Set(string key, ITransportable obj);
        public abstract void Delete(string key);
        public abstract void DeleteAll();


        public void Set(string key, int obj) => Set( key, new IntTransport() { value = obj } );
        public void Set(string key, float obj) => Set( key, new FloatTransport() { value = obj } );
        public void Set(string key, string obj) => Set( key, new StringTransport() { value = obj } );

        public bool GetInt(string key, out int result) {
            var r = new IntTransport();
            var b = GetValue( key, r );
            result = r.value;
            return b;
        }

        public bool GetFloat(string key, out float result) {
            var r = new FloatTransport();
            var b = GetValue( key, r );
            result = r.value;
            return b;
        }

        public bool GetString(string key, out string result) {
            var r = new StringTransport();
            var b = GetValue( key, r );
            result = r.value;
            return b;
        }




        [Serializable]
        abstract class BasicTransform<T> : ITransportable {
            public T value;
            public abstract string ToJson();
            public abstract void FromJson(string json);
        }

        [Serializable]
        class IntTransport : BasicTransform<int> {
            public override string ToJson() => value.ToString();
            public override void FromJson(string json) => value = JsonConvert.DeserializeObject<int>( json );
        }

        [Serializable]
        class FloatTransport : BasicTransform<float> {
            public override string ToJson() => value.ToString();
            public override void FromJson(string json) => value = JsonConvert.DeserializeObject<float>( json );
        }

        [Serializable]
        class StringTransport : BasicTransform<string> {
            public override string ToJson() => value;
            public override void FromJson(string json) => value = json;
        }
    }
}