using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace TowerDefense.Background {
    public class JsonConvertWarmup : MonoBehaviour {
        void OnEnable() {
            JsonConvert.DeserializeObject<JObject>( JsonConvert.SerializeObject( new {
                id = 12, str = "hi", nested = new {
                    floating = 1.2f
                }
            } ) );
            Destroy( this );
        }
    }
}