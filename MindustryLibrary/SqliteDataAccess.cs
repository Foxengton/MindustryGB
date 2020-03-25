using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;

namespace MindustryLibrary
{
	public class SqliteDataAccess
	{
		static SqliteDataAccess()
		{
			DBPath = "Data source=" + GetDatabasePath();
		}

		private static string GetDatabasePath()
		{
			return fileName;
		}

		private const string fileName = "MindustryDatabase.db";
		public static string DBPath;
	}
}