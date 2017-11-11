SQLite Manager
==============

SQLite database manager for Unity3D. Only helps query the database yet. To create your database, schema and add data I recommend to use [DB Browser for SQLite](http://sqlitebrowser.org/).

> It's under development, the syntax might change in the future. Even I try to make it safe please use it at your own risk.

## Usage example

Check out _Tests_ folder for usage example.

For example have a class:
```csharp
public class User
{
    public int UserID { get; set; }
    public string Name { get; set; }
}
```

Run native SQL query:
```csharp
using (IDbCommand dbcmd = db.Connection.CreateCommand()) {
    dbcmd.CommandText = "INSERT INTO `User` (`UserId`, `Name`) VALUES (1, 'John Doe'), (2, 'Jane Doe');";
    dbcmd.ExecuteNonQuery();
}
```

Init database:
```csharp
SQLiteManager db;
try {
    db = new SQLiteManager("Db/game.db");
} catch (SqliteException e) {
    Debug.Log("Sqlite exception: " + e.Message);
}
```

Get one row:
```csharp
User user = db.GetObj<User>(1);
User user = db.GetObj<User>("SELECT * FROM User WHERE UserID = 1");
```

Get multiple rows:
```csharp
List<User> users = db.GetObjList<User>("SELECT * FROM User");
```

## Recommended usage

It works well with [PersistData](https://github.com/SubZtep/PersistData) class.

Set database instance:
```csharp
PersistData.Instance.Set("db", db);
```

Get data:
```csharp
string data = ((SQLiteManager)PersistData.Instance.Get("db")).GetObj<User>(1).Name;
```