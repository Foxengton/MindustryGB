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
		public void Update()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Materials SET name = @Name, type = @Type, mod = @Mod, weight = @Weight, color = @Color WHERE id = @Id;", this);
			}

			if (!IsReset) General.Refresh(Id);
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM Materials WHERE id = @Id;", this);
			}

			General.Refresh(Id); //TODO: Обновление Generals после удаления материала.
		}

		public void Reset()
		{
			if (Name != "Copper" && Name != "Lead") Weight = null;
			else Weight = "1"; 

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Materials SET name = @Name, type = @Type, mod = @Mod, weight = @Weight, color = @Color WHERE id = @Id;", this);
			}
		}

		//===== VARIABLE =====//
		public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Mod { get; set; }
		public string Weight { get; set; }
		public string Color { get; set; }

		public override string ToString()
		{
			return $"{Id}. {Name} {Weight}";
		}

		public static void ResetAll(string id = "")
		{
			if (id == "")
			{
				IsReset = true;

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

				IsReset = false;
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
		public static Material GetMaterial(string id) => Materials.First(mat => mat.Id == id);
		public static Material[] GetAvailable() => Materials.Where(mat => mat.Weight != null).ToArray();

		public static Material[] Materials => Load();
		public static int Count => Materials.Count();
		public static string NextId => (Materials.Max(mat => Convert.ToInt32(mat.Id)) + 1).ToString();

		private static bool IsReset = false;
	}
}