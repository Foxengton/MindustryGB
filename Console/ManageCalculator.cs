using MindustryLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindustryConsole
{
	class ManageCalculator
	{
		public static void Menu()
		{
			int select;

			do
			{
				Console.WriteLine("╔═╤═╤═╡ SCHEMATIC MENU ╞═════╗");
				Console.WriteLine("║0├─┤ Exit                   ║");
				Console.WriteLine("║1├─┤ Find Item              ║");
				Console.WriteLine("║2├─┤ Check Schematic        ║");
				Console.WriteLine("╚═╧═╧════════════════════════╝");
				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0) return;
				else if (select == 1)
				{
					string value = ManageMaterial.SetItems(true);

					if (value != "" && value != null)
					{
						string item = value.Split(' ').First();
						double amount = Convert.ToDouble(value.Split(' ').Last());

						Cell cell = new Cell
						{
							outputId = item,
							outputAmount = amount
						};

						FindItem(cell, true);
					}
				}
				else if (select == 2) CheckShematic();
				else Formations.NotFound("Action");
			}
			while (true);
		}

		private static Cell FindItem(Cell cell, bool head = false)
		{
			//Search IO's output, which have target item as output
			for (int i = 0; i < InputOutput.InputsOutputs.Length; i++)
			{
				InputOutput inputOutput = InputOutput.InputsOutputs[i];
				double outputItem = inputOutput.GetOutput(cell.outputId);

				if (outputItem != 0)
				{
					General general = General.GetGeneral(inputOutput.GeneralId); //Owner IO
					double productionTime = Convert.ToDouble(inputOutput.ProductionTime); //IO's production time

					cell.mainAmount = Math.Ceiling(cell.outputAmount / (outputItem / productionTime) * 100) / 100; //Amount of factory needed for schematic
					cell.ece = Math.Round(cell.mainAmount / Math.Ceiling(cell.mainAmount) * 10000) / 100; //Energy conversion efficiency
					double max = Math.Round(Math.Ceiling(cell.mainAmount) * (outputItem / productionTime) * 100) / 100;

					string input = string.Empty;
					string main = Math.Ceiling(cell.mainAmount) + " " + general.Name;
					string output = (Math.Ceiling(cell.outputAmount * 100) / 100) + " " + Material.GetMaterial(cell.outputId).Name;

					if (inputOutput.Input != null)
					{
						cell.inputs = new List<Cell>();

						string[] items = inputOutput.Input.Split(';');

						for (int x = 0; x < items.Length; x++)
						{
							cell.inputs.Add(new Cell
							{
								outputId = items[x].Split(' ').First(),
								outputAmount = Convert.ToDouble(items[x].Split(' ').Last()) / productionTime * cell.mainAmount
							});

							input += cell.inputs[x].outputId + " " + (Math.Ceiling(cell.inputs[x].outputAmount * 100) / 100) + ";";

							if (cell.inputs[x].outputId == "20")
							{
								cell.power += cell.inputs[x].outputAmount;
							}
							else
							{
								cell.inputs[x] = FindItem(cell.inputs[x]);
								cell.power += cell.inputs[x].power;
							}
						}
					}

					if (input != string.Empty) Console.Write("{0} => ", ManageMaterial.NormalizateItems(input));
					Console.WriteLine("{0} => {1}/{2} ({3}%)", main, output, max, cell.ece);

					if (head)
					{
						Console.WriteLine("Power: {0}", cell.power);
						Console.WriteLine();
					}
				}
			}

			return cell;
		}

		private static void CheckShematic()
		{
			int select = 0;

			if (blockMaterials == null) blockMaterials = SetBlock();
			if (factories == null) factories = new List<Factory>();

			do
			{
				List<Summary> summaries = new List<Summary>();
				foreach (string io in blockMaterials)
					summaries.Add(new Summary { Id = io, Name = Material.GetMaterial(io).Name, Blocked = true});

				//===== SETUP VALUES ======//
				for (int i = 0; i < factories.Count; i++)
				{
					Factory factory = factories[i];
					Material material;
					double amount;
					factory.Ratio = 1;

					foreach (Item item in factory.Input)
					{
						amount = item.Amount * factory.Amount * factory.Ratio;
						material = Material.GetMaterial(item.Id);

						int index = summaries.FindIndex(x => x.Id == material.Id);
						if (index != -1) summaries[index].Outcome += amount;
						else summaries.Add(new Summary { Id = material.Id, Name = material.Name, Outcome = amount });
					}
					foreach (Item item in factory.Output)
					{
						amount = item.Amount * factory.Amount * factory.Ratio;
						material = Material.GetMaterial(item.Id);

						int index = summaries.FindIndex(x => x.Id == material.Id);
						if (index != -1) summaries[index].Income += amount;
						else summaries.Add(new Summary { Id = material.Id, Name = material.Name, Income = amount });
					}
				}

				//===== CORRECT SCHEMATIC ======//
				bool allCorrect;
				do
				{
					allCorrect = true;
					for (int i = 0; i < summaries.Count; i++)
					{
						Summary summary = summaries[i];

						//If summary is blocked or correct
						if (factories.Count < 2 || summary.Blocked || Math.Floor(summary.Income * 100000) - Math.Floor(summary.Outcome * 100000) == 0) continue;
						allCorrect = false;
						double result = summary.Income - summary.Outcome;

						//Get factories, where summary as i/o. Get factories' HashCode and created array.
						int[] factoryIO;
						if (result < 0) factoryIO = factories.Where(fc => fc.Input.Where(x => x.Id == summary.Id).Count() != 0).Select(str => str.GetHashCode()).ToArray();
						else factoryIO = factories.Where(fc => fc.Output.Where(x => x.Id == summary.Id).Count() != 0).Select(str => str.GetHashCode()).ToArray();

						foreach (int hash in factoryIO)
						{
							Summary[] summariesIO;
							Factory factory = factories.Find(f => f.GetHashCode() == hash); //Get factory

							if (summary.Outcome == 0) summary.Outcome = summary.Income;

							double ratio = summary.Income / summary.Outcome; //Set ratio

							//Get summaries, where summaries as Output.
							summariesIO = summaries.Where(sm => factory.Output.Where(fi => fi.Id == sm.Id).Count() != 0).ToArray();
							foreach (Summary summaryIO in summariesIO)
							{
								Item item = factory.Output.First(fi => fi.Id == summaryIO.Id); //Get summary's item
								double amount = factory.Ratio > 1 ? item.Amount * factory.Amount / factory.Ratio : item.Amount * factory.Amount * factory.Ratio; //Caolculate amount
								summaryIO.Income -= ratio > 1 ? amount - amount / ratio : amount - amount * ratio; //Edit Income
							}

							if (factory.Name != "Water Extractor" || result < 0)
							{
								//Get summaries, where summaries as Input.
								summariesIO = summaries.Where(sm => factory.Input.Where(fi => fi.Id == sm.Id).Count() != 0).ToArray();
								foreach (Summary summaryIO in summariesIO)
								{
									Item item = factory.Input.First(fi => fi.Id == summaryIO.Id); //Get summary's item
									double amount = factory.Ratio > 1 ? item.Amount * factory.Amount / factory.Ratio : item.Amount * factory.Amount * factory.Ratio; //Caolculate amount
									summaryIO.Outcome -= ratio > 1 ? amount - amount / ratio : amount - amount * ratio; //Edit Outcome
								}
							}

							//Reset ratio
							if (factory.Ratio == 1) factory.Ratio = ratio;
							else factory.Ratio *= ratio;
						}
					}
				}
				while (!allCorrect);

				//===== SHOW FACTORIES =====//
				foreach (Factory factory in factories)
				{
					double amount;
					Material material;

					Console.WriteLine("===== {0} ({1}) =====", factory.Name.ToUpper(), factory.Amount);
					foreach (Item item in factory.Input)
					{
						if (factory.Name != "Water Extractor") amount = factory.Ratio > 1 ? item.Amount * factory.Amount / factory.Ratio : item.Amount * factory.Amount * factory.Ratio;
						else amount = item.Amount * factory.Amount;
						material = Material.GetMaterial(item.Id);
						Console.WriteLine("- {0} {1}", amount, material.Name);
					}
					foreach (Item item in factory.Output)
					{
						amount = factory.Ratio > 1 ? item.Amount * factory.Amount / factory.Ratio : item.Amount * factory.Amount * factory.Ratio;
						material = Material.GetMaterial(item.Id);
						Console.WriteLine("+ {0} {1}", amount, material.Name);
					}
					Console.WriteLine();
				}

				//===== SHOW SUMMARIES =====//
				if (summaries.Count != 0)
				{
					Console.WriteLine("===== SUMMARY =====");
					foreach (Summary summary in summaries)
						Console.WriteLine("{0}: {1} - {2} = {3}", summary.Name, summary.Income, summary.Outcome, summary.Income - summary.Outcome);
					Console.WriteLine();
				}

				//===== SHOW RATIOS =====//
				double averageRatio = 0;
				Console.WriteLine("===== RATIOS =====");
				foreach (Factory factory in factories)
				{
					if (factory.Ratio > 1) averageRatio += factory.Amount / factory.Ratio / factory.Amount;
					else averageRatio += factory.Ratio;

					double amount = Math.Ceiling((factory.Ratio > 1 ? factory.Amount / factory.Ratio : factory.Amount * factory.Ratio) * 100) / 100;
					double whole = Math.Ceiling(amount);

					Console.WriteLine("{0} ({1})/{2} {3}", whole, amount, factory.Amount, factory.Name.ToUpper());
				}
				Console.WriteLine("AVERAGE RATIO: {0}%", Math.Round(averageRatio / factories.Count * 10000) / 100);
				Console.WriteLine();

				Console.WriteLine("╔═╤═╤═╡ CHECK SHEMATIC MENU ╞═════╗");
				Console.WriteLine("║0├─┤ Exit                        ║");
				Console.WriteLine("║1├─┤ Add Factory                 ║");
				Console.WriteLine("║2├─┤ Calculate                   ║");
				Console.WriteLine("╚═╧═╧═════════════════════════════╝");
				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0) return;
				else if (select == 1)
				{
					string factory = ManageBuilding.SetItems();

					if (factory != null && factory != "")
					{
						string id = factory.Split(' ').First();
						double amount = Convert.ToDouble(factory.Split(' ').Last());

						factories.Add(new Factory
						{
							Id = id,
							Name = General.GetGeneral(id).Name,
							Amount = Convert.ToDouble(amount),
							Input = InputOutput.GetGeneral(id).First().InputsPerSecond,
							Output = InputOutput.GetGeneral(id).First().OutputsPerSecond,
							Ratio = 1
						});
					}

					factories.ForEach(x => x.Ratio = 1);
				}
				else if (select == 2)
				{

				}
				else Formations.NotFound("Action");
			}
			while (true);
		}

		private static List<string> SetBlock()
		{
			List<string> blocks = new List<string>();
			int offset = 2;
			int select;

			do
			{
				Console.WriteLine("╔═╤═╤═╡ SCHEMATIC MENU ╞═════╗");
				Console.WriteLine("║0├─┤ Exit                   ║");
				Console.WriteLine("║1├─┤ Done                   ║");
				Console.WriteLine("╚═╧═╧════════════════════════╝");
				Console.WriteLine("┌────┬──────────────────────┬───────┐");
				Console.WriteLine("│ ID │ Name                 │ Block │");
				Console.WriteLine("├────┼──────────────────────┼───────┤");
				Material[] materials = Material.Materials.Where(mat => mat.Weight != null).ToArray();
				for (int i = 0; i < materials.Length; i++)
				{
					Material mat = materials[i];
					int id = i + offset;
					string name = mat.Name.PadRight(20, ' ');

					Console.WriteLine("│ {0,2} │ {1, 20} │ {2, 5} │", id, name, blocks.Contains(mat.Id) ? "Block" : "");
				}
				Console.WriteLine("└────┴──────────────────────┴───────┘");

				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0) return null;
				else if (select == 1) return blocks;
				else if (select >= offset && select < materials.Length + offset)
				{
					select -= offset;

					if (blocks.Contains(materials[select].Id))
						blocks.Remove(materials[select].Id);
					else
						blocks.Add(materials[select].Id);
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}

		public struct Cell
		{
			public List<Cell> inputs;
			public string mainId;
			public double mainAmount;
			public string outputId;
			public double outputAmount;
			public double ece;
			public double power;
		}

		private static List<Factory> factories;
		private static List<string> blockMaterials;

	}

	public class Factory
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public double Amount { get; set; }
		public Item[] Input { get; set; }
		public Item[] Output { get; set; }
		public double Ratio { get; set; }
	}

	public class Summary
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public double Income { get; set; }
		public double Outcome { get; set; }
		public bool Blocked { get; set; }
	}
}