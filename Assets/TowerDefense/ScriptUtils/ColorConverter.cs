using System;
using Newtonsoft.Json;
using UnityEngine;

namespace TowerDefense {
    public class ColorConverter : JsonConverter<Color> {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer) {
            writer.WriteValue( ColorUtility.ToHtmlStringRGBA( value ) );
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer) {
            Color col = Color.white;
            ColorUtility.TryParseHtmlString( reader.ReadAsString(), out col );
            return col;
        }
    }
}