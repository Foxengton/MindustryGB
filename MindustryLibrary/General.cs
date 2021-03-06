﻿using System;
using Dapper;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace MindustryLibrary
{
	public class General
	{
		#region//===== DATABASE =====//
		public static General[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
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
			} //If general already exist;

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"INSERT INTO Generals VALUES(@Id, @Name, @Description, @Type, @Health, @Size, @BuildTime, @BuildCost, @Mod, @Weight);", this);
			}
		}
		public void Update()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE Generals SET Name = @Name, Description = @Description, Type = @Type, Health = @Health, Size = @Size, BuildTime = @BuildTime, BuildCost = @BuildCost, Mod = @Mod, Weight = @Weight WHERE Id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"DELETE FROM Generals WHERE Id = @Id;", this);
			}
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

		public static General[] Generals => Load();
		public Item[] BuildCosts
		{
			get
			{
				if (BuildCost == null) return null;
				string[] items = BuildCost.Split(';');
				List<Item> buildCost = new List<Item>();

				foreach (string item in items)
				{
					if (item == "") return null;

					buildCost.Add(new Item
					{
						Id = item.Split(' ').First(),
						Amount = Convert.ToDouble(item.Split(' ').Last())
					});
				}

				return buildCost.ToArray();
			}
		}

		public static string DBPath { get; set; }
		#endregion

		#region//===== OTHER FUNCTION =====//
		public static General GetGeneral(string id) => Generals.Count(gen => gen.Id == id) != 0 ? Generals.First(gen => gen.Id == id) : null;
		public InputOutput[] GetInputOutputs => InputOutput.InputsOutputs.Where(io => io.GeneralId == Id).ToArray();
		public Power[] GetPowers => Power.Powers.Where(power => power.GeneralId == Id).ToArray();
		public Mod GetMod => Mod == null ? null : MindustryLibrary.Mod.GetMod(Mod);
		public TypeItem GetTypeMaterial => Type == null ? null : TypeItem.GetType(Type);

		public static int Count => Generals.Count();
		public static string NextId => Count == 0 ? "0" : (Generals.Max(gen => Convert.ToInt32(gen.Id)) + 1).ToString();

		#endregion

		#region//===== OVERRIDES =====//
		public override string ToString()
		{
			return $"{Name} ({string.Join(", ", BuildCosts.Select(bc => bc.ToString()))})";
		}
		#endregion

		//TODO: расчёт веса и операции с весом
		#region//===== WEIGHT =====//
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

		public void Reset()
		{
			Weight = null;

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE Generals SET name = @Name, description = @Description, type = @Type, health = @Health, size = @Size, buildTime = @BuildTime, buildCost = @BuildCost, mod = @Mod, weight = @Weight WHERE id = @Id;", this);
			}
		}
		public static void ResetAll()
		{
			foreach (General general in Generals)
				general.Reset();
		}

		public static void Refresh(string materialId = "")
		{
			if (materialId == "") for (int i = 0; i < Generals.Length; i++) Generals[i].Save();
			else
			{
				List<General> generals = Generals.Where(gen => gen.BuildCosts.Count(item => item.Id == materialId) != 0 && gen.BuildCosts.Count(item => Material.GetMaterial(item.Id).Weight == null) == 0).ToList();

				foreach (General general in generals)
					general.Update();
			}
		}
		#endregion
	}
}