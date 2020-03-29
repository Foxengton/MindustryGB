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
				Console.WriteLine("║2├─┤ Calculate              ║");
				Console.WriteLine("╚═╧═╧════════════════════════╝");
				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0) return;
				else if (select == 1)
				{
					string value = ManageMaterial.SetItems(true, true);

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
				else if (select == 2) continue; //CalculateSchematic();
				else Formations.NotFound("Action");
			}
			while (true);
		}

		private static void CalculateSchematic()
		{
			List<Factory> factories = new List<Factory>();

			do
			{
				string item = ManageBuilding.SetItems();

				if (item != "" && item != null)
				{
					factories.Add(new Factory
					{
						Id = item.Split(' ').First(),
						Name = General.GetGeneral(item.Split(' ').First()).Name,
						Amount = Convert.ToDouble(item.Split(' ').Last()),
					});

					InputOutput[] inputOutputs = InputOutput.GetGeneral(factories.Last().Id);

					if (inputOutputs.Length == 0) Formations.NotFound("Input/Output");
					else if (inputOutputs.Length == 1)
					{
						factories.Last().Input = inputOutputs[0].Inputs;
						if (factories.Last().Input != null)
						{
							double productionTime = Convert.ToDouble(inputOutputs[0].ProductionTime);

							for (int i = 0; i < factories.Last().Input.Length; i++)
								factories.Last().Input[i].Amount /= productionTime;
						}

						factories.Last().Output = inputOutputs[0].Outputs;
						if (factories.Last().Output != null)
						{
							double productionTime = Convert.ToDouble(inputOutputs[0].ProductionTime);

							for (int i = 0; i < factories.Last().Output.Length; i++)
								factories.Last().Output[i].Amount /= productionTime;
						}
					}
				}
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

		private struct Cell
		{
			public List<Cell> inputs;
			public string mainId;
			public double mainAmount;
			public string outputId;
			public double outputAmount;
			public double ece;
			public double power;
		}
	}

	public class Factory
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public double Amount { get; set; }
		public Item[] Input { get; set; }
		public Item[] Output { get; set; }
		public int Power { get; set; }
	}
}