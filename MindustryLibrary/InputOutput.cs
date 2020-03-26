using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace MindustryLibrary
{
	public class InputOutput
	{
		public static InputOutput[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				return cnn.Query<InputOutput>("SELECT * FROM InputsOutputs;", new DynamicParameters()).ToArray();
			}
		}
		public void Save()
		{
			CalculateWeight();

			if (inputsOutputs.Where(io => io.Id == Id).Count() != 0)
			{
				Update();
				return;
			}

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"INSERT INTO InputsOutputs VALUES(@Id, @GeneralId, @Input, @Output, @ProductionTime, @Weight);", this);
			}

			inputsOutputs = Load();
		}
		public void Update()
		{
			if (Input != inputsOutputs.Where(io => io.Id == Id).ToArray()[0].Input)
				CalculateWeight();
			else if (ProductionTime != inputsOutputs.Where(io => io.Id == Id).ToArray()[0].ProductionTime)
				CalculateWeight();

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE InputsOutputs SET generalId = @GeneralId, input = @Input, output = @Output, productionTime = @ProductionTime, weight = @Weight WHERE id = @Id;", this);
			}

			inputsOutputs = Load();
		}
		public void Delete(bool generalDeleted = false)
		{
			if (!generalDeleted)
			{
				string weight = CalculateWeight().ToString();

				for (int i = 0; i < Material.materials.Length; i++)
					if (Material.materials[i].Weight == weight)
						Material.Reset();
			}

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM InputsOutputs WHERE id = @Id;", this);
			}

			inputsOutputs = Load();
		}

		private double CalculateWeight()
		{
			double inputWeight = 0;
			double baseWeight = 0;
			double totalWeight = 0;

			//If owner have not a weight
			if (General.generals.Where(gen => gen.Id == GeneralId).ToArray()[0].Weight == null)
				return 0;
			else
				baseWeight = Convert.ToDouble(General.generals.Where(gen => gen.Id == GeneralId).ToArray()[0].Weight);

			if (Input != null)
			{
				string[] input = Input.Split(';'); //Get items

				for (int i = 0; i < Material.materials.Length; i++)
				{
					//Current material not available
					if (input[i] != "" && Material.materials[i].Weight == null)
					{
						Weight = null;
						return 0;
					}

					//Set input weight
					if (input[i] != "")
						inputWeight += Convert.ToDouble(input[i]) / Convert.ToDouble(ProductionTime) * Convert.ToDouble(Material.materials[i].Weight);
				}
			} //Input not null

			if (Output != null)
			{
				string[] output = Output.Split(';'); //Get items

				for (int i = 0; i < Material.materials.Length; i++)
				{
					// If output is able
					if (output[i] != "")
					{	
						double multiplier = 1 / (Convert.ToDouble(output[i]) / Convert.ToDouble(ProductionTime));

						inputWeight *= multiplier;
						baseWeight *= multiplier;

						totalWeight = Math.Round((inputWeight + baseWeight) * 100) / 100;

						Weight = (Math.Round(inputWeight * 100) / 100).ToString(); //Set weight
						Material.CheckWeight(i, totalWeight); //Check weight
						return totalWeight;
					}
				}
			} //Output not null

			return 0;
		}
		public static void AllRefresh(string id, bool save = true)
		{
			for (int i = 0; i < inputsOutputs.Length; i++)
				if (inputsOutputs[i].GeneralId == id && save)
					inputsOutputs[i].Save();
				else if(inputsOutputs[i].GeneralId == id && !save)
					inputsOutputs[i].Delete(true);
		}

		public string Id { get; set; }
		public string GeneralId { get; set; }
		public string Input { get; set; }
		public string Output { get; set; }
		public string ProductionTime { get; set; }
		public string Weight { get; set; }

		public static InputOutput[] inputsOutputs = Load();

		public static string NextId { get => inputsOutputs.Length == 0 ? "1" : (Convert.ToInt32(inputsOutputs[inputsOutputs.Length - 1].Id) + 1).ToString(); }
	}
}