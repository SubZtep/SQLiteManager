using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DemoLand.SQLiteManager
{
	[CustomEditor(typeof(ManageDatabase))]
	public class ManageDatabaseEditor : Editor
	{
		public override void OnInspectorGUI() {
			ManageDatabase md = (ManageDatabase)target;

			//DrawDefaultInspector();
			GUILayout.Space(8);
			Texture2D tex = Resources.Load("LabelText") as Texture2D;
			Texture2D bgtex = Resources.Load("LabelBg") as Texture2D;
			GUIStyle style = new GUIStyle();
			style.normal.background = bgtex;
			style.alignment = TextAnchor.MiddleCenter;
			GUILayout.Box(tex, style, new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Height(20) });
			GUILayout.Space(3);
			GUILayout.Box(
				"Please find SQLite specific buttons here, I still suggesst to visit "+
				"http://sqlitebrowser.org/ and download their app for proper navigation.",
				new GUIStyle("HelpBox")
			);
			GUILayout.Space(3);
			GUILayout.BeginVertical("box");

			if (GUILayout.Button("Test database connection")) {
				bool success = md.Test();
				string message = success ? "Success, your database is ready to use." : "Something went wrong, check console for details.";
				string button = success ? "Yay!" : "Boo!";
				EditorUtility.DisplayDialog("Test Database", message, button);
			}
			GUILayout.Space(2);
			if (GUILayout.Button("Print row count for all tables")) {
				md.PrintTableCount();
			}
			GUILayout.Space(2);
			if (GUILayout.Button("Truncate all tables")) {
				if (EditorUtility.DisplayDialog(
					"Wipe Down All The Data",
					"Are you sure you want to delete all database records? No backsies!",
					"Truncate",
					"Cancel"
				)) {
					md.Truncate();
				}
			}

			GUILayout.EndVertical();
			GUILayout.Space(2);
		}
	}
}
