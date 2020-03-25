using Dapper;
using System.Data.SQLite;
using System.Linq;
using System.Data;
using System;

namespace MindustryLibrary
{
	public class Material
	{
		public static Material[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				return cnn.Query<Material>("SELECT * FROM Materials;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			if (materials.Where(mat => mat.Id == Id).Count() != 0)
			{
				Update();
				return;
			} //If material already exists;

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"INSERT INTO Materials VALUES(@Id, @Name, @Type, @Mod, @Weight, @Color);", this);
			}

			materials = Load(); //Update local library
		}
		public void Update(bool refresh = true)
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE Materials SET name = @Name, type = @Type, mod = @Mod, weight = @Weight, color = @Color WHERE id = @Id;", this);
			}

			materials = Load(); //Update local library

			if (refresh)
			{
				int index = GetIndex();
				General.AllRefresh(index);
			}
		}
		public void Delete()
		{
			int index = GetIndex(); //index material in local library

			//Update all generals;
			for (int i = 0; i < General.generals.Length; i++)
				if (General.generals[i].BuildCost != null)
					General.generals[i].BuildCost = UpdateItems(General.generals[i].BuildCost, index); //Set new items
			
			//Update all inputsOutputs;
			for (int i = 0; i < InputOutput.inputsOutputs.Length; i++)
			{
				if (InputOutput.inputsOutputs[i].Input != null)
					InputOutput.inputsOutputs[i].Input = UpdateItems(InputOutput.inputsOutputs[i].Input, index); //Set new items
				if (InputOutput.inputsOutputs[i].Output != null)
					InputOutput.inputsOutputs[i].Output = UpdateItems(InputOutput.inputsOutputs[i].Output, index); //Set new items
			}

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM Materials WHERE id = @Id;", this);
			}

			materials = Load(); //Update local library

			//Resave all generals weight;
			for (int i = 0; i < General.generals.Length; i++)
				General.generals[i].Update();

			//Resave all inputsOutputs;
			for (int i = 0; i < InputOutput.inputsOutputs.Length; i++)
				InputOutput.inputsOutputs[i].Update();
		}

		public static void Reset()
		{
			for (int i = 0; i < materials.Length; i++)
				if (materials[i].Name != "Copper" && materials[i].Name != "Lead")
					materials[i].Weight = null;

			General.AllRefresh(-1);
		}
		public static void CheckWeight(int index, double weight)
		{
			if (materials[index].Weight == null)
			{
				materials[index].Weight = weight.ToString();
				materials[index].Update(false);
			}
			else if (Convert.ToDouble(materials[index].Weight) > weight)
			{
				materials[index].Weight = weight.ToString();
				materials[index].Update();
			}
		}
		private string UpdateItems(string itemsString, int index)
		{
			string[] items = itemsString.Split(';'); //Get array of items
			string result = string.Empty;

			//Set all items except deleted
			for (int i = 0; i < materials.Length; i++)
				if (i != index)
					result += items[i] + ";";

			return result; //Return new items
		}
		private int GetIndex()
		{
			for (int i = 0; i < materials.Length; i++)
				if (materials[i].Id == Id)
					return i;
			return -1;
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public string Mod { get; set; }
		public string Weight { get; set; }
		public string Color { get; set; }

		public static Material[] materials = Load();

		public static string NextId { get => (Convert.ToInt32(materials[materials.Length - 1].Id) + 1).ToString(); }

	}
}