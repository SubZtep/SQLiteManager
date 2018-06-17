using System.Collections;
using UnityEngine;

namespace DemoLand.PersistData
{
	/// <summary>
	/// Store data between scenes,
	/// like to stay on its own gameobject.
	/// </summary>
	[DisallowMultipleComponent]
	[AddComponentMenu("Miscellaneous/Persist Data")]
	public class PersistData : Singleton<PersistData>
	{
		private Hashtable data = new Hashtable();

		/// <summary>
		/// Set data in the private hashtable
		/// </summary>
		/// <param name="key">Object key</param>
		/// <param name="obj">Payload</param>
		/// <returns></returns>
		public void Set(object key, object obj) {
			if (data.ContainsKey(key)) {
				data[key] = obj;
			} else {
				data.Add(key, obj);
			}
		}

		/// <summary>
		/// Get data from the private hashtable
		/// </summary>
		/// <param name="key">Object key</param>
		/// <returns></returns>
		public object Get(object key) {
			if (data.ContainsKey(key)) {
				return data[key];
			}
			return null;
		}

		/// <summary>
		/// Delete data from the private hashtable
		/// </summary>
		/// <param name="key">Object key</param>
		public void Del(object key) {
			if (data.ContainsKey(key)) {
				data.Remove(key);
			}
		}

		/// <summary>
		/// Check data in the private hashtable
		/// </summary>
		/// <param name="key">Object key</param>
		/// <returns>Is exists?</returns>
		public bool Has(object key) {
			return data.ContainsKey(key);
		}
	}
}
