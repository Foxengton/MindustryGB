using System;
using Dapper;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Collections.Generic;

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
			if (Generals.Count(gen => gen.Id == Id) != 0)
			{
				Update();
				return;
			}

			CalculateWeight();

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"INSERT INTO Generals VALUES(@Id, @Name, @Description, @Type, @Health, @Size, @BuildTime, @BuildCost, @Mod, @Weight);", this);
			}

			InputOutput.GetGeneral(Id).ToList().ForEach(fe => fe.Update());
		}
		public void Update()
		{
			CalculateWeight();
			if (Weight == null) return;

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Generals SET name = @Name, description = @Description, type = @Type, health = @Health, size = @Size, buildTime = @BuildTime, buildCost = @BuildCost, mod = @Mod, weight = @Weight WHERE id = @Id;", this);
			}

			InputOutput.GetGeneral(Id).ToList().ForEach(fe => fe.Update());
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM Generals WHERE id = @Id;", this);
			}

			InputOutput.InputsOutputs.Where(io => io.GeneralId == Id).ToList().ForEach(fe => fe.Delete());
		}

		public void Reset()
		{
			Weight = null;

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Generals SET name = @Name, description = @Description, type = @Type, health = @Health, size = @Size, buildTime = @BuildTime, buildCost = @BuildCost, mod = @Mod, weight = @Weight WHERE id = @Id;", this);
			}
		}

		public static void ResetAll()
		{
			foreach (General general in Generals)
				general.Reset();
		}

		//===== DATABASE'S VARIABLES =====//
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

		//===== LOCAL VARIABLES =====//
		public Item[] BuildCosts
		{
			get
			{
				if (BuildCost == null) return null;
				string[] items = BuildCost.Split(';');
				List<Item> buildCost = new List<Item>();

				foreach (string item in items)
				{
					buildCost.Add(new Item {
						Id = item.Split(' ').First(),
						Amount = Convert.ToDouble(item.Split(' ').Last())
					});
				}

				return buildCost.ToArray();
			}
		}

		public override string ToString()
		{
			return $"{Id}. {Name} {Weight} [{string.Join(", ", BuildCosts.Select(bc => bc.ToString()))}]";
		}

		private double CalculateWeight()
		{
			Weight = null;
			double weight = 0;

			for (int i = 0; i < BuildCosts.Length; i++)
			{
				string materialWeight = Material.GetMaterial(BuildCosts[i].Id).Weight; //Get id of material

				if (materialWeight == null) return weight;

				weight += BuildCosts[i].Amount * Convert.ToDouble(materialWeight);
			} //Calculate weight

			weight = Math.Round(weight * 100) / 100;
			Weight = weight.ToString();
			return weight;
		}

		public static General GetGeneral(string id) => Generals.First(gen => gen.Id == id);
		public static void Refresh(string materialId = "")
		{
			if (materialId == "") for (int i = 0; i < Generals.Length; i++) Generals[i].Save();
			else
			{
				List<General> generals = Generals.Where(gen => gen.BuildCosts.Count(bc => bc.Id == materialId) != 0).ToList();

				foreach (General general in generals)
					general.Update();
			}
		}

		public static General[] Generals => Load();
		public static int Count => Generals.Count();
		public static string NextId => (Generals.Max(gen => Convert.ToInt32(gen.Id)) + 1).ToString();
	}
}