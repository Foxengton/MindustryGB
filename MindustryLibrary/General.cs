using System;
using Dapper;
using System.Data.SQLite;
using System.Data;
using System.Linq;

namespace MindustryLibrary
{
	public class General
	{
		//===== WORKS WITH DATABASE =====//
		public static General[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				return cnn.Query<General>("SELECT * FROM Generals ORDER BY Mod, Type, Name;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			CalculateWeight();

			if (Generals.Where(gen => gen.Id == Id).Count() != 0)
			{
				Update();
				return;
			}

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"INSERT INTO Generals VALUES(@Id, @Name, @Description, @Type, @Health, @Size, @BuildTime, @BuildCost, @Mod, @Weight);", this);
			}
		}
		public void Update()
		{
			if (BuildCost != Generals.Where(gen => gen.Id == Id).ToArray()[0].BuildCost)
				CalculateWeight();

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Generals SET name = @Name, description = @Description, type = @Type, health = @Health, size = @Size, buildTime = @BuildTime, buildCost = @BuildCost, mod = @Mod, weight = @Weight WHERE id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM Generals WHERE id = @Id;", this);
			}

			InputOutput.AllRefresh(Id, false);
		}

		//===== VARIABLE =====//
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

		private void CalculateWeight()
		{
			Weight = null;
			string[] buildCost = BuildCost.Split(';');
			double weight = 0;

			for (int i = 0; i < buildCost.Length; i++)
			{
				string materialWeight = Material.GetMaterial(buildCost[i].Split(' ').First()).Weight; //Get id of material
				double amount = Convert.ToDouble(buildCost[i].Split(' ').Last()); //Get amount of material

				if (materialWeight == null) return;

				weight += amount * Convert.ToDouble(materialWeight);
			} //Calculate weight

			Weight = (Math.Round(weight * 100) / 100).ToString();

			InputOutput.AllRefresh(Id);
		}


		public static General GetGeneral(string id) => Generals.First(gen => gen.Id == id);
		public static void Refresh(string materialId = "-1")
		{
			if (materialId == "-1")
			{
				for (int i = 0; i < Generals.Length; i++)
					Generals[i].Save();
			}
			else
			{
				for (int i = 0; i < Generals.Length; i++)
				{
					string[] items = Generals[i].BuildCost.Split(';');

					for (int x = 0; x < items.Length; x++)
						if (items[x].Split(' ').First() == materialId)
							Generals[i].Save();
				}
			}
		}

		public static General[] Generals => Load();
		public static int Count => Generals.Count();
		public static string NextId => (Convert.ToInt32(Generals.Max(gen => Convert.ToInt32(gen.Id))) + 1).ToString();
	}
}