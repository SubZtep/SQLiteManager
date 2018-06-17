using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DemoLand.SQLiteManager
{
	[CustomEditor(typeof(UseDatabase))]
	public class UseDatabaseEditor : Editor
	{
		private SerializedProperty streamingAsset;
		private SerializedProperty filePath;

		void OnEnable() {
			streamingAsset = serializedObject.FindProperty("streamingAsset");
			filePath = serializedObject.FindProperty("filePath");
		}

		public override void OnInspectorGUI() {
			serializedObject.Update();
			EditorGUILayout.PropertyField(streamingAsset, new GUIContent("SQLite Binary"));
			EditorGUILayout.HelpBox(
				"Please select the binary SQLite database file from the StreamingAssets folder.",
				MessageType.None,
				false
			);

			if (streamingAsset.objectReferenceValue == null) {
				return;
			}

			filePath.stringValue = AssetDatabase.GetAssetPath(streamingAsset.objectReferenceValue.GetInstanceID());
			serializedObject.ApplyModifiedProperties();
		}
	}
}
