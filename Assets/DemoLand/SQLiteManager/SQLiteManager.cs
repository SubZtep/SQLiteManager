using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Collections;

namespace DemoLand.SQLiteManager
{
	public class SQLiteManager
	{
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
			string conn = "URI=file:"+System.IO.Path.Combine(Application.streamingAssetsPath, saDbPath);
			dbconn = (IDbConnection)new SqliteConnection(conn);
			dbconn.Open();
		}

		public void Close() {
			dbconn.Close();
			dbconn = null;
		}

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

		public T GetObj<T>(int objId) where T : class, new() {
			string objName = typeof(T).Name;
			string sql = "SELECT * FROM "+objName+" WHERE "+objName+"ID = @id LIMIT 1";

			Dictionary<string, object> parameters = new Dictionary<string, object>();
			parameters["@id"] = objId;
			
			return GetObj<T>(sql, parameters);
		}

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

		private T BuildObj<T>(IDataReader reader) where T : class, new() {
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
					Debug.Log("Error: " + e.Message);
					continue;
				}
			}

			return obj;
		}

		public bool ColumnExists(IDataReader reader, string columnName) {
			return reader.GetSchemaTable()
					.Rows
					.OfType<DataRow>()
					.Any(row => row["ColumnName"].ToString() == columnName);
		}
	}
}