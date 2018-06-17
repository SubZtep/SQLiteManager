using DemoLand.PersistData;
using DemoLand.SQLiteManager;

public enum PDKey : short {
	Database
}

public class PD
{
	public static SQLiteManager Database() {
		return (SQLiteManager)PersistData.Instance.Get(PDKey.Database);
	}
}
