using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace TowerDefense.ScriptUtils
{
	public class SubTypeJsonConverter<T> : JsonConverter
	{
		class SpecifiedConcreteClassConverter<K> : DefaultContractResolver
		{
			protected override JsonConverter ResolveContractConverter(Type objectType) {
				if ( typeof(K).IsAssignableFrom( objectType ) && !objectType.IsAbstract )
					return null; // pretend it is not specified (thus avoiding a stack overflow)
				return base.ResolveContractConverter( objectType );
			}
		}

		static JsonSerializerSettings _settings = new()
			{ ContractResolver = new SpecifiedConcreteClassConverter<T>() };

		// all subclasses 
		static Type[] types =
			(from assemblyDomain in AppDomain.CurrentDomain.GetAssemblies()
				from type in assemblyDomain.GetTypes()
				where !type.IsAbstract && (type.IsSubclassOf( typeof(T) ) || type == typeof(T))
				select type).ToArray();

		static string[] typeNames = types.Select( t => {
			var attr = t.GetCustomAttributes<DisplayNameAttribute>().ToArray();
			return attr.Length > 0 ? attr[0].DisplayName : t.Name;
		} ).ToArray();



		static string typeFieldName = "type";

		static FieldInfo typeFI = typeof(T).GetField( typeFieldName,
			BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Default );

		JsonSerializer _jsonSerializer = JsonSerializer.Create( _settings );

		public override bool CanConvert(Type objectType) => objectType == typeof(T);

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			if ( value is not T t_vaue ) return;
			for ( int i = 0; i < types.Length; i++ ) {
				if ( types[i] == t_vaue.GetType() ) {
					typeFI.SetValue( t_vaue, typeNames[i] );
					break;
				}
			}

			_jsonSerializer.Serialize( writer, value );
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			if ( reader.TokenType == JsonToken.Null ) return null;
			var jo = JObject.Load( reader );
			var type = jo[typeFieldName].Value<string>();
			for ( int i = 0; i < typeNames.Length; i++ ) {
				if ( typeNames[i] == type ) {
					return jo.ToObject( types[i], _jsonSerializer );
				}
			}

			throw new Exception( type );
		}
	}
}