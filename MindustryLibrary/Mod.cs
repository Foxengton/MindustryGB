using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace MindustryLibrary
{
	public class Mod
	{
		#region//===== DATABASE =====//
		public static Mod[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				return cnn.Query<Mod>("SELECT * FROM Mods;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			if (Mods.Count(mod => mod.Id == Id) != 0)
			{
				Update();
				return;
			}

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"INSERT INTO Mods VALUES(@Id, @Name, @Version);", this);
			}
		}
		public void Update()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE Mods SET Name = @Name, Version = @Version WHERE Id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"DELETE FROM Mods WHERE Id = @Id;", this);
			}
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Version { get; set; }

		public static Mod[] Mods => Load();

		public static string DBPath { get; set; }
		#endregion

		#region//===== OTHER FUNCTION =====//
		public static Mod GetMod(string id) => Mods.Count(power => power.Id == id) != 0 ? Mods.First(power => power.Id == id) : null;

		public static int Count => Mods.Count();
		public static string NextId => Count == 0 ? "0" : (Mods.Max(power => Convert.ToInt32(power.Id)) + 1).ToString();
		#endregion

		#region//===== OVERRIDES =====//
		public override string ToString()
		{
			return $"{Name} v{Version}";
		}
		#endregion
	}
}
