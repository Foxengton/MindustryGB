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
		public InputOutput(){}

		public InputOutput(string generalId)
		{
			Id = NextId;
			GeneralId = generalId;
		}

		#region//===== DATABASE =====//
		public static InputOutput[] Load()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
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
			} //If InputsOutputs already exist;

			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"INSERT INTO InputsOutputs VALUES(@Id, @GeneralId, @Input, @Output, @ProductionTime, @Weight, @MaterialWeight);", this);
			}
		}
		public void Update()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"UPDATE InputsOutputs SET GeneralId = @GeneralId, Input = @Input, Output = @Output, ProductionTime = @ProductionTime, Weight = @Weight, MaterialWeight = @MaterialWeight WHERE Id = @Id;", this);
			}
		}
		public void Delete()
		{
			using (IDbConnection cnn = new SQLiteConnection(DBPath))
			{
				cnn.Execute($"DELETE FROM InputsOutputs WHERE Id = @Id;", this);
			}
		}

		public string Id { get; set; }
		public string GeneralId { get; set; }
		public string Input { get; set; }
		public string Output { get; set; }
		public string ProductionTime { get; set; }
		public string Weight { get; set; }
		public string MaterialWeight { get; set; }

		public static InputOutput[] InputsOutputs => Load();
		public Item[] Inputs
		{
			get
			{
				if (Input == null) return null;
				string[] items = Input.Split(';');
				List<Item> input = new List<Item>();

				foreach (string item in items)
				{
					if (item == "") return null;
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
					if (item == "") return null;

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
					if (item == "") return null;

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
					if (item == "") return null;

					output.Add(new Item
					{
						Id = item.Split(' ').First(),
						Amount = Convert.ToDouble(item.Split(' ').Last()) / Convert.ToDouble(ProductionTime)
					});
				}

				return output.ToArray();
			}
		}

		public static string DBPath { get; set; }
		#endregion

		#region//===== OTHER FUNCTION =====//
		public static InputOutput GetInputOutput(string id) => InputsOutputs.First(io => io.Id == id);
		public General GetGeneral => General.GetGeneral(GeneralId);

		public static int Count => InputsOutputs.Count();
		public static string NextId => Count == 0 ? "0" : (InputsOutputs.Max(io => Convert.ToInt32(io.Id)) + 1).ToString();
		#endregion

		#region//===== OVERRIDES =====//
		public override string ToString()
		{
			string input = "null";
			string output = "null";

			if (Inputs != null)
				input = string.Join(", ", InputsPerSecond.Select(bc => bc.ToString()));

			if (Outputs != null)
				output = string.Join(", ", OutputsPerSecond.Select(bc => bc.ToString()));

			return $"{GetGeneral.Name}: {input} => {output}";
		}
		#endregion

		//TODO: расчёт веса и операции с весом
		#region//===== WEIGHT =====//
		public static void Refresh(string materialId = "")
		{
			if (materialId == "") for (int i = 0; i < InputsOutputs.Length; i++) InputsOutputs[i].Update();
			else
			{
				InputOutput[] inputOutputs;

				inputOutputs = InputsOutputs.Where(io => (io.Inputs != null && io.Inputs.Count(bc => bc.Id == materialId) != 0) || (io.Outputs != null && io.Outputs.Count(bc => bc.Id == materialId) != 0)).ToArray();
				foreach (InputOutput inputOutput in inputOutputs)
				{
					Item[] items;

					items = inputOutput.Inputs.Where(item => Material.GetMaterial(item.Id) != null).ToArray();
					inputOutput.Input = string.Join(";", items.Select(item => item.Id + " " + item.Amount));
					if (inputOutput.Input == "") inputOutput.Input = null;

					items = inputOutput.Outputs.Where(item => Material.GetMaterial(item.Id) != null).ToArray();
					inputOutput.Output = string.Join(";", items.Select(item => item.Id + " " + item.Amount));
					if (inputOutput.Output == "") inputOutput.Output = null;

					inputOutput.Update();
				}
			}
		}

		private void CalculateWeight()
		{
			double inputWeight = 0;
			double baseWeight;
			double totalWeight;

			//If owner have not a weight
			if (GetGeneral.Weight == null) return;
			else baseWeight = Convert.ToDouble(GetGeneral.Weight);

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
			else
			{
				double ratio = 1 / (1 / Convert.ToDouble(ProductionTime));

				inputWeight *= ratio;
				baseWeight *= ratio;

				totalWeight = Math.Round((inputWeight + baseWeight) * 100) / 100;

				MaterialWeight = totalWeight.ToString();

				Weight = (Math.Round(inputWeight * 100) / 100).ToString(); //Set weight
			}
		}
		#endregion
	}
}