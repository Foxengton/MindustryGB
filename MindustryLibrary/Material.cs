using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace MindustryLibrary
{
	public class Material
	{
		#region//===== DATABASE =====//
		public static Material[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
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
			} //If material already exist;

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"INSERT INTO Materials VALUES(@Id, @Name, @Type, @Mod, @Weight, @Color);", this);
			}
		}
		public void Update()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE Materials SET Name = @Name, Type = @Type, Mod = @Mod, Weight = @Weight, Color = @Color WHERE Id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"DELETE FROM Materials WHERE Id = @Id;", this);
			}
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Mod { get; set; }
		public string Weight { get; set; }
		public string Color { get; set; }

		public static Material[] Materials => Load();

		public static string DBPath { get; set; }
		#endregion

		#region//===== OTHER FUNCTION =====//
		public static Material GetMaterial(string id) => Materials.Count(mat => mat.Id == id) != 0 ? Materials.First(mat => mat.Id == id) : null;
		public static Material[] GetAvailable => Materials.Where(mat => mat.Weight != null).ToArray();
		public Mod GetMod => Mod == null ? null : MindustryLibrary.Mod.GetMod(Mod);

		public static int Count => Materials.Count();
		public static string NextId => Count == 0 ? "0" : (Materials.Max(mat => Convert.ToInt32(mat.Id)) + 1).ToString();
		#endregion

		#region//===== OVERRIDES =====//
		public override string ToString()
		{
			return $"{Id}. {Name} {Weight}";
		}
		#endregion

		//TODO: расчёт веса и операции с весом
		#region//===== WEIGHT =====//
		public void Reset()
		{
			if (Name != "Copper" && Name != "Lead") Weight = null;
			else Weight = "1";

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE Materials SET name = @Name, type = @Type, mod = @Mod, weight = @Weight, color = @Color WHERE id = @Id;", this);
			}
		}
		public static void ResetAll(string id = "")
		{
			if (id == "")
			{
				foreach (Material material in Materials)
					material.Reset();

				General.ResetAll();
				General[] generals = General.Generals.Where(gen => gen.Weight == null).ToArray();
				do
				{
					foreach (General general in generals)
						general.Update();

					generals = General.Generals.Where(gen => gen.Weight == null).ToArray();
				}
				while (generals.Length != 0);
			}
			else if (GetMaterial(id).Weight != null)
			{
				Material material = GetMaterial(id);
				material.Weight = null;
				material.Update();
			}
		}

		public static void CheckWeight(string id, double weight)
		{
			if (GetMaterial(id).Weight == null || Convert.ToDouble(GetMaterial(id).Weight) > weight)
			{
				Material material = GetMaterial(id);
				material.Weight = weight.ToString();
				material.Update();
			}
		}
		#endregion
	}
}