using System;
using System.Collections.Generic;
using UnityEngine;

namespace DemoLand.SQLiteManager
{
	[AddComponentMenu("SQLite Manager/Manage Database")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UseDatabase))]
	public class ManageDatabase : MonoBehaviour
	{
		/// <summary>
		/// Try to connect to the local database
		/// </summary>
		/// <returns>Is it success?</returns>
		public bool Test() {
			UseDatabase udb = GetComponent<UseDatabase>();
			string dbFile = udb.GetDatabaseFilePath();
			if (dbFile.Length == 0) {
				Debug.LogError("No database file selected, please assign in the inspector window!");
				return false;
			}
			try {
				new SQLiteManager(dbFile);
				return true;
			} catch (Exception e) {
				udb.UnselectBinary();
				Debug.LogException(e);
			}
			return false;
		}

		/// <summary>
		/// Print row count of all tables to the console
		/// </summary>
		public void PrintTableCount() {
			SQLiteManager db = GetDb();
			List<string> names = db.GetTableNames();
			foreach (string name in names) {
				string sql = "SELECT COUNT(*) AS cnt FROM " + name;
				print(name + ": " + db.GetResultReader(sql)["cnt"]);
			}
		}

		/// <summary>
		/// Delete all data (be careful with it, make secure copy)
		/// </summary>
		public void Truncate() {
			SQLiteManager db = GetDb();
			List<string> names = db.GetTableNames();
			foreach (string name in names) {
				db.TruncateTable(name);
			}
		}

		private SQLiteManager GetDb() {
			UseDatabase udb = GetComponent<UseDatabase>();
			SQLiteManager db = null;
			try {
				db = new SQLiteManager(udb.GetDatabaseFilePath());
			} catch (Exception e) {
				udb.UnselectBinary();
				Debug.LogException(e);
				throw e;
			}
			return db;
		}
	}
}
