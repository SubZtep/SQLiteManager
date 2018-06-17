using UnityEngine;

namespace DemoLand.SQLiteManager
{
	[AddComponentMenu("SQLite Manager/Use Database")]
	[DisallowMultipleComponent]
	public class UseDatabase : MonoBehaviour
	{
		#pragma warning disable 0414
		[SerializeField]
		private Object streamingAsset;
		#pragma warning restore 0414

		[SerializeField]
		private string filePath;

		public string GetDatabaseFilePath() {
			if (string.IsNullOrEmpty(filePath)) {
				throw new System.NullReferenceException("SQLite Binary is unselected in the Inspector window");
			}
			string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
			return System.IO.Path.Combine(path, filePath);
		}

		/// <summary>
		/// Clear inspector from useless database file.
		/// </summary>
		public void UnselectBinary() {
			streamingAsset = null;
			filePath = null;
		}
	}
}
