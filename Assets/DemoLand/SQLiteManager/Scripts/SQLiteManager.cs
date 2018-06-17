using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;
using System.Linq;

namespace DemoLand.SQLiteManager
{
	public class SQLiteManager
	{
		#region Connection

		private IDbConnection dbconn = null;

		public IDbConnection Connection {
			get { return dbconn; }
		}

		public SQLiteManager(string saDbPath = null) {
			if (saDbPath != null) {
				Connect(saDbPath);
			}
		}

		~SQLiteManager() {
			Close();
		}

		public void Connect(string saDbPath) {
			dbconn = (IDbConnection)new SqliteConnection("URI=file:" + saDbPath);
			dbconn.Open();
		}

		public void Close() {
			dbconn.Close();
			dbconn = null;
		}

		public bool IsOpen() {
			return dbconn != null && dbconn.State == ConnectionState.Open;
		}

		#endregion

		#region Getter Methods

		/// <summary>
		/// Query only one row from the database
		/// </summary>
		/// <typeparam name="T">Type (Object with database table fields)</typeparam>
		/// <param name="sql">SQL command</param>
		/// <param name="parameters">Dictionary with SQL criteria values</param>
		/// <returns>Object with data or null</returns>
		public T GetObj<T>(string sql, Dictionary<string, object> parameters = null) where T : class, new() {
			T obj = null;
			using (IDbCommand dbcmd = dbconn.CreateCommand()) {
				dbcmd.CommandText = sql;

				if (parameters != null) {
					foreach (KeyValuePair<string, object> param in parameters) {
						IDbDataParameter parameter = dbcmd.CreateParameter();
						parameter.ParameterName = param.Key;
						parameter.Value = param.Value;
						dbcmd.Parameters.Add(parameter);
					}
				}

				using (IDataReader reader = dbcmd.ExecuteReader()) {
					if (reader.Read()) {
						obj = BuildObj<T>(reader);
					}
				}
			}
			return obj;
		}

		/// <summary>
		/// Query only one row from the database
		/// </summary>
		/// <typeparam name="T">Type (Object with database table fields)</typeparam>
		/// <param name="objId">Dictionary with SQL criteria values</param>
		/// <returns>Object with data or null</returns>
		public T GetObj<T>(int objId) where T : class, new() {
			string objName = typeof(T).Name;
			string sql = "SELECT * FROM "+objName+" WHERE "+objName+"ID = @id LIMIT 1";

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["@id"] = objId;

			return GetObj<T>(sql, parameters);
		}

		/// <summary>
		/// Query multiple rows from the database
		/// </summary>
		/// <typeparam name="T">Type (Object with database table fields)</typeparam>
		/// <param name="sql">SQL command</param>
		/// <param name="parameters">Dictionary with SQL criteria values</param>
		/// <returns>Object list with data or null</returns>
		public List<T> GetObjList<T>(string sql, Dictionary<string, object> parameters = null) where T : class, new() {
			List<T> objs = new List<T>();
			using (IDbCommand dbcmd = dbconn.CreateCommand()) {
				dbcmd.CommandText = sql;

				if (parameters != null) {
					foreach (KeyValuePair<string, object> param in parameters) {
						IDbDataParameter parameter = dbcmd.CreateParameter();
						parameter.ParameterName = param.Key;
						parameter.Value = param.Value;
						dbcmd.Parameters.Add(parameter);
					}
				}

				using (IDataReader reader = dbcmd.ExecuteReader()) {
					while (reader.Read()) {
						T obj = BuildObj<T>(reader);
						objs.Add(obj);
					}
				}
			}
			return objs;
		}

		/// <summary>
		/// Get all the tables in the database
		/// </summary>
		/// <returns>List of tables</returns>
		public List<string> GetTableNames() {
			List<string> names = new List<string>();
			using (IDbCommand dbcmd = Connection.CreateCommand()) {
				dbcmd.CommandText = string.Format("SELECT name FROM sqlite_master WHERE type='table';");
				using (IDataReader reader = dbcmd.ExecuteReader()) {
					while (reader.Read()) {
						names.Add(reader["name"].ToString());
					}
				}
			}
			return names;
		}

		public IDataReader GetResultReader(string sql, Dictionary<string, object> parameters = null) {
			using (IDbCommand dbcmd = dbconn.CreateCommand()) {
				dbcmd.CommandText = sql;
				if (parameters != null) {
					foreach (KeyValuePair<string, object> param in parameters) {
						IDbDataParameter parameter = dbcmd.CreateParameter();
						parameter.ParameterName = param.Key;
						parameter.Value = param.Value;
						dbcmd.Parameters.Add(parameter);
					}
				}
				return dbcmd.ExecuteReader();
			}
		}

		#endregion

		#region Data Manipulation Methods

		/// <summary>
		/// Save object into the database. If ID column populated update otherwise insert it.
		/// </summary>
		/// <typeparam name="T">Type (Object with database table fields)</typeparam>
		/// <param name="obj">Object with data</param>
		public void SaveObj<T>(T obj) {
			Type myType = obj.GetType();
			IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());
			//bool insert = props[0].GetValue(obj) == null;
			bool insert = true;

			string sql = insert ? "INSERT INTO {0} " : "UPDATE {0} SET ";
			sql = String.Format(sql, myType.Name);

			string[] fields = new string[props.Count-1];
			object[] values = new object[props.Count-1];

			for (int i = 0; i < props.Count - 1; i++) {
				PropertyInfo prop = props[i+1];
				fields[i] = prop.Name;
				values[i] = prop.GetValue(obj, null);
			}

			if (insert) {
				sql += "(" + String.Join(", ", fields) + ") VALUES (@" + String.Join(", @", fields) + ")";
			} else {
				for (int i = 0; i < fields.Length; i++) {
					if (i > 0) sql += ", ";
					sql += fields[i] + " = @" + fields[i];
				}
				sql += " WHERE " + myType.Name + "ID = @" + myType.Name + "ID";
			}

			using (IDbCommand dbcmd = dbconn.CreateCommand()) {
				dbcmd.CommandText = sql;
				for (int i = 0; i < fields.Length; i++) {
					IDbDataParameter parameter = dbcmd.CreateParameter();
					parameter.ParameterName = fields[i];
					parameter.Value = values[i];
					dbcmd.Parameters.Add(parameter);
				}
				if (!insert) {
					IDbDataParameter parameter = dbcmd.CreateParameter();
					parameter.ParameterName = myType.Name + "ID";
					//parameter.Value = props[0].GetValue(obj);
					dbcmd.Parameters.Add(parameter);
				}
				dbcmd.ExecuteNonQuery();
			}
		}

		/// <summary>
		/// Permanently delete all the record from a table.
		/// </summary>
		/// <param name="tableName">Table for wipe the data</param>
		public void TruncateTable(string tableName) {
			using (IDbCommand dbcmd = Connection.CreateCommand()) {
				dbcmd.CommandText = string.Format("DELETE FROM {0};", tableName);

				int rowsAffected = dbcmd.ExecuteNonQuery();
				Debug.Log(tableName + ": " + rowsAffected + " rows affected.");

				dbcmd.CommandText = "VACUUM;";
				dbcmd.ExecuteNonQuery();
			}
		}

		#endregion

		#region Helper Functions

		/// <summary>
		/// Instantiate an object and populate data from raw database result
		/// </summary>
		/// <typeparam name="T">Type (Object with database table fields)</typeparam>
		/// <param name="reader">SQLite result</param>
		/// <returns>Object with data</returns>
		public T BuildObj<T>(IDataReader reader) where T : class, new() {
			T obj = new T();

			foreach (var prop in obj.GetType().GetProperties()) {
				try {
					if (ColumnExists(reader, prop.Name)) {
						PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);

						if (reader[prop.Name] == DBNull.Value) {
							propertyInfo.SetValue(obj, null, null);
						} else {
							Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
							object safeValue = reader[prop.Name] == null ? null : Convert.ChangeType(reader[prop.Name], t);
							propertyInfo.SetValue(obj, safeValue, null);
						}
					}
				} catch (Exception e) {
					Debug.LogException(e);
					continue;
				}
			}

			return obj;
		}

		/// <summary>
		/// Tells if provided column is exists in the db result
		/// </summary>
		/// <param name="reader">SQLite result</param>
		/// <param name="columnName">Db column / Obj property name</param>
		/// <returns>Is exists?</returns>
		public bool ColumnExists(IDataReader reader, string columnName) {
			return reader.GetSchemaTable()
					.Rows
					.OfType<DataRow>()
					.Any(row => row["ColumnName"].ToString() == columnName);
		}

		/// <summary>
		/// Static method that parse sql, helps during debug
		/// </summary>
		/// <param name="sql">SQL command</param>
		/// <param name="parameters">Dictionary with SQL criteria values</param>
		/// <returns>Runnable SQL</returns>
		public static string GetParsedSql(string sql, Dictionary<string, object> parameters = null) {
			if (parameters != null) {
				foreach (KeyValuePair<string, object> param in parameters) {
					sql = sql.Replace(param.Key, param.Value.ToString());
				}
			}
			return sql;
		}

		#endregion
	}
}
