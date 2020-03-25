using System;
using Dapper;
using System.Data.SQLite;
using System.Data;
using System.Linq;

namespace MindustryLibrary
{
	public class General
	{
		public static General[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				return cnn.Query<General>("SELECT * FROM Generals;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			CalculateWeight();

			if (generals.Where(gen => gen.Id == Id).Count() != 0)
			{
				Update();
				return;
			}

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
				{
					cnn.Execute($"INSERT INTO Generals VALUES(@Id, @Name, @Description, @Type, @Health, @Size, @BuildTime, @BuildCost, @Mod, @Weight);", this);
				}

			generals = Load();
		}
		public void Update()
		{
			if (BuildCost != generals.Where(gen => gen.Id == Id).ToArray()[0].BuildCost)
				CalculateWeight();

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Generals SET name = @Name, description = @Description, type = @Type, health = @Health, size = @Size, buildTime = @BuildTime, buildCost = @BuildCost, mod = @Mod, weight = @Weight WHERE id = @Id;", this);
			}

			generals = Load();
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM Generals WHERE id = @Id;", this);
			}

			generals = Load();
		}

		private void CalculateWeight()
		{
			string[] buildCost = BuildCost.Split(';');
			double weight = 0;

			for (int i = 0; i < Material.materials.Length; i++)
			{
				if (buildCost[i] != "" && Material.materials[i].Weight == null)
				{
					Weight = null;
					return;
				}

				if (buildCost[i] != "")
					weight += Convert.ToDouble(buildCost[i]) * Convert.ToDouble(Material.materials[i].Weight);
			}

			Weight = (Math.Round(weight * 100) / 100).ToString();

			InputOutput.AllRefresh(Id);
		}
		public static void AllRefresh(int newMaterial = -1)
		{
			if (newMaterial != -1)
			{
				for (int i = 0; i < generals.Length; i++)
					if (generals[i].BuildCost.Split(';')[newMaterial] != "")
						generals[i].Save();
			}
			else
				for (int i = 0; i < generals.Length; i++)
					generals[i].Save();
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string Type { get; set; }
		public string Health { get; set; }
		public string Size { get; set; }
		public string BuildTime { get; set; }
		public string BuildCost { get; set; }
		public string Mod { get; set; }
		public string Weight { get; set; }

		public static General[] generals = Load();
		public static string NextId { get => generals.Length == 0 ? "1" : (Convert.ToInt32(generals[generals.Length - 1].Id) + 1).ToString(); }
	}
}