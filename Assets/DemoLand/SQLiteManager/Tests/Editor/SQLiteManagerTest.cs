using UnityEngine;
using NUnit.Framework;
using Mono.Data.Sqlite;
using System.Data;
using System.Collections.Generic;

namespace DemoLand.SQLiteManager.Test
{
	public class SQLiteManagerTest
	{
		private SQLiteManager db;

		[Test]
		public void SQLiteManagerTestSimplePasses() {
			Assert.True(SetupTestDb(), "Connect to database");
			SetupTestData();

			User user = db.GetObj<User>(1);
			Assert.AreEqual(user.Name, "John Doe", "Test data");

			List<User> users = db.GetObjList<User>("SELECT * FROM User");
			Assert.AreEqual(users.Count, 2, "Rows count");

			ClearTestData();
		}

		private bool SetupTestDb() {
			try {
				db = new SQLiteManager("Test.db");
			} catch (SqliteException e) {
				Debug.Log("Sqlite exception: " + e.Message);
				return false;
			}
			return true;
		}

		private void SetupTestData() {
			using (IDbCommand dbcmd = db.Connection.CreateCommand()) {
				try {
					dbcmd.CommandText = "CREATE TABLE `User` (`UserId` INTEGER PRIMARY KEY, `Name` TEXT);";
					dbcmd.ExecuteNonQuery();
				} catch (SqliteException) {
					dbcmd.CommandText = "DELETE FROM `User`;";
					dbcmd.ExecuteNonQuery();
				}
				dbcmd.CommandText = "INSERT INTO `User` (`UserId`, `Name`) VALUES (1, 'John Doe'), (2, 'Jane Doe');";
				dbcmd.ExecuteNonQuery();
			}
		}

		public void ClearTestData() {
			using (IDbCommand dbcmd = db.Connection.CreateCommand()) {
				dbcmd.CommandText = "DROP TABLE `User`;";
				dbcmd.ExecuteNonQuery();
			}
		}
	}
}
