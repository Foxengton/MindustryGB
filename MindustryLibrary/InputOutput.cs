using Dapper;
using System;
using System.Collections.Generic;
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
			if (InputsOutputs.Count(io => io.Id == Id) != 0)
			{
				Update();
				return;
			}

			CalculateWeight();

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"INSERT INTO InputsOutputs VALUES(@Id, @GeneralId, @Input, @Output, @ProductionTime, @Weight, @MaterialWeight);", this);
			}
		}
		public void Update()
		{
			CalculateWeight();

			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"UPDATE InputsOutputs SET generalId = @GeneralId, input = @Input, output = @Output, productionTime = @ProductionTime, weight = @Weight, materialWeight = @MaterialWeight WHERE id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(SqliteDataAccess.DBPath))
			{
				cnn.Execute($"DELETE FROM InputsOutputs WHERE id = @Id;", this);
			}

			foreach (Item item in Outputs)
				Material.ResetAll(item.Id);
		}

		//===== DATABASE'S VARIABLES =====//
		public string Id { get; set; }
		public string GeneralId { get; set; }
		public string Input { get; set; }
		public string Output { get; set; }
		public string ProductionTime { get; set; }
		public string Weight { get; set; }
		public string MaterialWeight { get; set; }

		//===== LOCAL VARIABLES =====//
		public Item[] Inputs
		{
			get
			{
				if (Input == null) return null;
				string[] items = Input.Split(';');
				List<Item> input = new List<Item>();

				foreach (string item in items)
				{
					input.Add(new Item
					{
						Id = item.Split(' ').First(),
						Amount = Convert.ToDouble(item.Split(' ').Last())
					});
				}

				return input.ToArray();
			}
		}
		public Item[] Outputs
		{
			get
			{
				if (Output == null) return null;
				string[] items = Output.Split(';');
				List<Item> output = new List<Item>();

				foreach (string item in items)
				{
					output.Add(new Item
					{
						Id = item.Split(' ').First(),
						Amount = Convert.ToDouble(item.Split(' ').Last())
					});
				}

				return output.ToArray();
			}
		}
		public Item[] InputsPerSecond
		{
			get
			{
				if (Input == null) return null;
				string[] items = Input.Split(';');
				List<Item> input = new List<Item>();

				foreach (string item in items)
				{
					input.Add(new Item
					{
						Id = item.Split(' ').First(),
						Amount = Convert.ToDouble(item.Split(' ').Last()) / Convert.ToDouble(ProductionTime)
					});
				}

				return input.ToArray();
			}
		}
		public Item[] OutputsPerSecond
		{
			get
			{
				if (Output == null) return null;
				string[] items = Output.Split(';');
				List<Item> output = new List<Item>();

				foreach (string item in items)
				{
					output.Add(new Item
					{
						Id = item.Split(' ').First(),
						Amount = Convert.ToDouble(item.Split(' ').Last()) / Convert.ToDouble(ProductionTime)
					});
				}

				return output.ToArray();
			}
		}

		public override string ToString()
		{
			return $"{General.GetGeneral(GeneralId)}: {string.Join(", ", Inputs.Select(bc => bc.ToString()))} => {string.Join(", ", Outputs.Select(bc => bc.ToString()))} [{Weight}]";
		}


		private void CalculateWeight()
		{
			double inputWeight = 0;
			double baseWeight;
			double totalWeight;

			//If owner have not a weight
			if (General.GetGeneral(GeneralId).Weight == null) return;
			else baseWeight = Convert.ToDouble(General.GetGeneral(GeneralId).Weight);

			if (Input != null)
			{
				for (int i = 0; i < Inputs.Length; i++)
				{
					double materialWeight = Convert.ToDouble(Material.GetMaterial(Inputs[i].Id).Weight); //Get weight of material

					inputWeight += Inputs[i].Amount / Convert.ToDouble(ProductionTime) * materialWeight; //Get weight of input
				}
			} //Input not null
			if (Output != null)
			{
				for (int i = 0; i < Outputs.Length; i++)
				{
					double ratio = 1 / (Outputs[i].Amount / Convert.ToDouble(ProductionTime));

					inputWeight *= ratio;
					baseWeight *= ratio;

					totalWeight = Math.Round((inputWeight + baseWeight) * 100) / 100;

					MaterialWeight = totalWeight.ToString();

					Weight = (Math.Round(inputWeight * 100) / 100).ToString(); //Set weight
					Material.CheckWeight(Outputs[i].Id, totalWeight); //Check weight
				}
			} //Output not null
		}

		public static InputOutput GetInputOutput(string id) => InputsOutputs.First(io => io.Id == id);
		public static InputOutput[] GetGeneral(string generalId) => InputsOutputs.Where(io => io.GeneralId == generalId).ToArray();
		public static InputOutput[] InputsOutputs => Load();
		public static string NextId => (InputsOutputs.Max(io => Convert.ToInt32(io.Id)) + 1).ToString();
	}
}