
using Newtonsoft.Json;
using UnityEngine;

namespace TowerDefense.Transport
{
	public interface ITransportable {
		public string ToJson() => JsonConvert.SerializeObject( this );
		public void FromJson(string json) => JsonConvert.PopulateObject( json, this );
	}

	public interface IDataExchange<in T> {
		public void ApplyTo(T target);
		public void TakeFrom(T source);
	}

	public interface IValidCheck {
		public bool IsValid();
	}

	public static class TransportExtensions {
		public static T Cloned<T>(this T value) where T : ITransportable, new() {
			var json = value.ToJson();
			var obj = new T();
			obj.FromJson( json );
			return obj;
		}
		
		/// <summary>
		/// clone for scriptable objects
		/// </summary>
		/// <returns></returns>
		public static T ClonedSO<T>(this T value) where T : ScriptableObject, ITransportable, new() {
			var json = value.ToJson();
			var obj = ScriptableObject.CreateInstance<T>();
			obj.FromJson( json );
			return obj;
		}
	}
}