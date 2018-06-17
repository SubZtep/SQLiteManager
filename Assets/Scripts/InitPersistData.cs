using DemoLand.PersistData;
using DemoLand.SQLiteManager;
using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(UseDatabase))]
public class InitPersistData : MonoBehaviour
{
	private void Awake()
	{
		InitDatabaseConnection();
	}

	private void InitDatabaseConnection()
	{
		if (!PersistData.Instance.Has(PDKey.Database)) {
			SQLiteManager db = null;
			UseDatabase udb = GetComponent<UseDatabase>();

			try {
				db = new SQLiteManager(udb.GetDatabaseFilePath());
			} catch (Exception e) {
				udb.UnselectBinary();
				Debug.LogException(e);
			}

			if (db.IsOpen()) {
				PersistData.Instance.Set(PDKey.Database, db);
			}
		}
	}
}
