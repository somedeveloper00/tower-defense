using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TowerDefense {
    public class ColorConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => objectType == typeof(Color);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteValue( ColorUtility.ToHtmlStringRGBA( (Color)value ) );
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer) {
            Color col = Color.white;
            ColorUtility.TryParseHtmlString( reader.ReadAsString(), out col );
            return col;
        }
    }
}