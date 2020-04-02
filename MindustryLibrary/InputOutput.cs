using Dapper;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace MindustryLibrary
{
	public class InputOutput
	{
		//===== WORKS WITH DATABASE =====//
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

			if (InputsOutputs.Where(io => io.Id == Id).Count() != 0)
			{
				Update();
				return;
			}

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"INSERT INTO InputsOutputs VALUES(@Id, @GeneralId, @Input, @Output, @ProductionTime, @Weight);", this);
			}

		}
		public void Update()
		{
			if (Input != InputsOutputs.Where(io => io.Id == Id).ToArray()[0].Input)
				CalculateWeight();
			else if (ProductionTime != InputsOutputs.Where(io => io.Id == Id).ToArray()[0].ProductionTime)
				CalculateWeight();

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE InputsOutputs SET generalId = @GeneralId, input = @Input, output = @Output, productionTime = @ProductionTime, weight = @Weight WHERE id = @Id;", this);
			}
		}
		public void Delete(bool generalDeleted = false)
		{
			if (!generalDeleted)
			{
				string weight = CalculateWeight().ToString();

				for (int i = 0; i < Material.Count; i++)
					if (Material.Materials[i].Weight == weight)
						Material.Reset();
			}

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM InputsOutputs WHERE id = @Id;", this);
			}
		}

		//===== DATABASE'S VARIABLES =====//
		public string Id { get; set; }
		public string GeneralId { get; set; }
		public string Input { get; set; }
		public string Output { get; set; }
		public string ProductionTime { get; set; }
		public string Weight { get; set; }

		//===== LOCAL VARIABLES =====//
		public Item[] Inputs
		{
			get
			{
				if (Input == null) return null;
				string[] items = Input.Split(';');
				Item[] input = new Item[items.Length];

				for (int i = 0; i < input.Length; i++)
				{
					input[i].Id = items[i].Split(' ').First();
					input[i].Amount = Convert.ToDouble(items[i].Split(' ').Last());
				}

				return input;
			}
		}
		public Item[] Outputs
		{
			get
			{
				if (Output == null) return null;
				string[] items = Output.Split(';');
				Item[] output = new Item[items.Length];

				for (int i = 0; i < output.Length; i++)
				{
					output[i].Id = items[i].Split(' ').First();
					output[i].Amount = Convert.ToDouble(items[i].Split(' ').Last());
				}

				return output;
			}
		}
		public Item[] InputsPerSecond
		{
			get
			{
				if (Input == null) return null;
				string[] items = Input.Split(';');
				Item[] input = new Item[items.Length];

				for (int i = 0; i < input.Length; i++)
				{
					input[i].Id = items[i].Split(' ').First();
					input[i].Amount = Convert.ToDouble(items[i].Split(' ').Last()) / Convert.ToDouble(ProductionTime);
				}

				return input;
			}
		}
		public Item[] OutputsPerSecond
		{
			get
			{
				if (Output == null) return null;
				string[] items = Output.Split(';');
				Item[] output = new Item[items.Length];

				for (int i = 0; i < output.Length; i++)
				{
					output[i].Id = items[i].Split(' ').First();
					output[i].Amount = Convert.ToDouble(items[i].Split(' ').Last()) / Convert.ToDouble(ProductionTime);
				}

				return output;
			}
		}

		public double GetInput(string id) => Material.GetItem(Input, id);
		public double GetOutput(string id) => Material.GetItem(Output, id);
		private double CalculateWeight()
		{
			double inputWeight = 0;
			double baseWeight;
			double totalWeight;

			//If owner have not a weight
			if (General.GetGeneral(GeneralId).Weight == null) return 0;
			else baseWeight = Convert.ToDouble(General.GetGeneral(GeneralId).Weight);

			if (Input != null)
			{
				string[] input = Input.Split(';'); //Get items

				for (int i = 0; i < input.Length; i++)
				{
					double materialWeight = Convert.ToDouble(Material.GetMaterial(input[i].Split(' ').First()).Weight); //Get id of material
					double amount = Convert.ToDouble(input[i].Split(' ').Last()); //Get amount of material

					inputWeight += amount / Convert.ToDouble(ProductionTime) * materialWeight; //Get weight of input
				}
			} //Input not null

			if (Output != null)
			{
				double amount = Convert.ToDouble(Output.Split(' ').Last());

				double multiplier = 1 / (amount / Convert.ToDouble(ProductionTime));

				inputWeight *= multiplier;
				baseWeight *= multiplier;

				totalWeight = Math.Round((inputWeight + baseWeight) * 100) / 100;

				Weight = (Math.Round(inputWeight * 100) / 100).ToString(); //Set weight
				Material.CheckWeight(Output.Split(' ').First(), totalWeight); //Check weight
				return totalWeight;

			} //Output not null

			return 0;
		}

		public static void AllRefresh(string id, bool save = true)
		{
			for (int i = 0; i < InputsOutputs.Length; i++)
				if (InputsOutputs[i].GeneralId == id && save)
					InputsOutputs[i].Save();
				else if(InputsOutputs[i].GeneralId == id && !save)
					InputsOutputs[i].Delete(true);
		}
		public static InputOutput GetInputOutput(string id) => InputsOutputs.First(io => io.Id == id);
		public static InputOutput[] GetGeneral(string generalId) => InputsOutputs.Where(io => io.GeneralId == generalId).ToArray();
		public static InputOutput[] InputsOutputs => Load();
		public static string NextId => (Convert.ToInt32(InputsOutputs.Max(io => Convert.ToInt32(io.Id))) + 1).ToString();
	}
}