using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace MindustryLibrary
{
	public class Material
	{
		//===== WORKS WITH DATABASE =====//
		public static Material[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				return cnn.Query<Material>("SELECT * FROM Materials ORDER BY Mod, Type, Name;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			if (Materials.Count(mat => mat.Id == Id) != 0)
			{
				Update();
				return;
			} //If material already exists;

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"INSERT INTO Materials VALUES(@Id, @Name, @Type, @Mod, @Weight, @Color);", this);
			}
		}
		public void Update(bool refresh = true)
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Materials SET name = @Name, type = @Type, mod = @Mod, weight = @Weight, color = @Color WHERE id = @Id;", this);
			}

			if (refresh) General.Refresh(Id);
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM Materials WHERE id = @Id;", this);
			}

			General.Refresh();
		}

		//===== VARIABLE =====//
		public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Mod { get; set; }
		public string Weight { get; set; }
		public string Color { get; set; }


		public static double GetItem(string item, string id)
		{
			string[] items = item.Split(';');

			for (int i = 0; i < items.Length; i++)
				if (items[i].Split(' ').First() == id)
					return Convert.ToDouble(items[i].Split(' ').Last());

			return 0;
		}
		public static void Reset()
		{
			for (int i = 0; i < Count; i++)
				if (Materials[i].Name != "Copper" && Materials[i].Name != "Lead")
				{
					Materials[i].Weight = null;
					Materials[i].Update();
				}

			General.Refresh();
		}
		public static void CheckWeight(string id, double weight)
		{
			if (GetMaterial(id).Weight == null || Convert.ToDouble(GetMaterial(id).Weight) > weight)
				GetMaterial(id).Update(false);
		}
		public static Material GetMaterial(string id) => Materials.First(mat => mat.Id == id);
		public static Material[] GetAvailable() => Materials.Where(mat => mat.Weight != null).ToArray();

		public static Material[] Materials => Load();
		public static int Count => Materials.Count();
		public static string NextId => (Convert.ToInt32(Materials.Max(mat => Convert.ToInt32(mat.Id))) + 1).ToString();
	}
}