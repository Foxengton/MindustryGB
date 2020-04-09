using MindustryLibrary;

namespace MindustryConsole
{
	public class SqliteDataAccess
	{
		public static void DataAccess()
		{
			DBPath = "Data source=" + GetDatabasePath();

			Material.DBPath = DBPath;
			General.DBPath = DBPath;
			Power.DBPath = DBPath;
			InputOutput.DBPath = DBPath;
		}

		private static string GetDatabasePath()
		{
			return fileName;
		}

		private const string fileName = "MindustryDatabase.db";
		public static string DBPath;
	}
}