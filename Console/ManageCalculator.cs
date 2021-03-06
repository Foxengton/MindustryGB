﻿using MindustryLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindustryConsole
{
	class ManageCalculator
	{
		public static void Menu()
		{
			FindItem();

			//char select;

			//do
			//{
			//	Console.WriteLine("╔═╤╤══╡ SCHEMATIC MENU ╞═════╗");
			//	Console.WriteLine("║0├┤ Exit                    ║");
			//	Console.WriteLine("║1├┤ Find Item               ║");
			//	Console.WriteLine("╚═╧╧═════════════════════════╝");
			//	Console.Write("> ");
			//	select = Console.ReadKey().KeyChar;
			//	Console.Clear();

			//	if (select == '0')
			//		return;
			//	else if (select == '1')
			//		FindItem();
			//	else
			//		Formations.NotFound("Action");
			//}
			//while (true);
		}

		private static void FindItem()
		{
			Item targetItem = ManageMaterial.SetItems(true).First(); //TODO заменить функцию выбора предмета.
			List<Summary> summaries = new List<Summary> { new Summary
			{
				Id = targetItem.Id,
				Outcome = targetItem.Amount,
				Name = Material.GetMaterial(targetItem.Id).Name //TODO добавить в Item функцию получения материала.
			}};

			List<Factory> factories = new List<Factory>();

			bool repeat;
			do
			{
				repeat = false;

				for (int s = 0; s < summaries.Count; s++)
				{
					if (summaries[s].Blocked || summaries[s].Conveyor) continue;

					Summary summary = summaries[s];
					double ratio = summary.Income - summary.Outcome;

					if (ratio < 0)
					{
						repeat = true;

						//===== INPUT/OUTPUT BLOCK =====//
						int inputOutputsIndex = 0;
						InputOutput[] inputOutputs = InputOutput.InputsOutputs.Where(io => io.Outputs != null && io.Outputs.Count(it => it.Id == summary.Id) != 0).ToArray();
						
						if (inputOutputs.Length == 0)
							inputOutputsIndex = -1;
						else if (inputOutputs.Length > 0)
						{
							int offset = 2;
							int select;

							do
							{
								Console.WriteLine("╔═╤═╤═╡ SELECT FACTORY ({0}) ╞═════", summary.Name.ToUpper());
								Console.WriteLine("║0├─┤ Exit");
								Console.WriteLine("║1├─┤ Conveyor");
								for (int io = 0; io < inputOutputs.Length; io++)
									Console.WriteLine("║{0}├─┤ {1}", io + offset, inputOutputs[io].ToString());
								Console.WriteLine("╚═╧═╧════════════════════════");
								Console.Write("> ");
								select = Formations.GetInt(Console.ReadKey().KeyChar.ToString());
								Console.Clear();

								if (select == 0)
									return;
								else if (select == 1)
								{
									inputOutputsIndex = -1;
									break;
								}
								else if (select >= offset && select <= inputOutputs.Length + offset)
								{
									inputOutputsIndex = select - offset;
									break;
								}
								else
									Formations.NotFound("Action");
							}
							while (true);
						}
						
						if (inputOutputsIndex == -1)
						{
							double select = 0;

							Console.WriteLine("═════╡ ENTER AMOUNT OF {0} ╞═════", summary.Name.ToUpper());
							Console.Write("> ");
							select = Formations.GetDouble(Console.ReadLine());
							Console.Clear();

							if (select <= 0)
								return;
							
							summary.Income = select;
							summary.Conveyor = true;

							continue;
						}

						InputOutput inputOutput = inputOutputs[inputOutputsIndex];
						General general = inputOutput.GetGeneral;

						//===== FACTORY BLOCK =====//
						Factory factory = new Factory {
							Id = general.Id,
							Name = general.Name,
							Amount = Math.Ceiling(Math.Abs(ratio) / inputOutput.OutputsPerSecond.Where(ops => ops.Id == summary.Id).First().Amount),
							Input = inputOutput.InputsPerSecond,
							Output = inputOutput.OutputsPerSecond,
							Ratio = 1
						};

						summaries = EditSummaries(summaries, factory);

						if (factories.Count(fc => fc.Id == factory.Id) != 0)
						{
							Factory existFactory = factories.Where(fc => fc.Id == factory.Id).First();

							existFactory.Amount += factory.Amount;
						}
						else
							factories.Add(factory);
					}
				}
			}
			while (repeat);

			CorrectSchematic(summaries, factories);
		}

		private static List<Summary> EditSummaries(List<Summary> summaries, Factory factory)
		{
			double amount;
			Material material;

			if (factory.Input != null)
				foreach (Item item in factory.Input)
				{
					amount = item.Amount * factory.Amount * factory.Ratio;
					material = Material.GetMaterial(item.Id);

					int index = summaries.FindIndex(x => x.Id == material.Id);
					if (index != -1)
						summaries[index].Outcome += amount;
					else
						summaries.Add(new Summary { Id = material.Id, Name = material.Name, Outcome = amount });
				}
			if (factory.Output != null)
				foreach (Item item in factory.Output)
				{
					amount = item.Amount * factory.Amount * factory.Ratio;
					material = Material.GetMaterial(item.Id);

					int index = summaries.FindIndex(x => x.Id == material.Id);
					if (index != -1)
						summaries[index].Income += amount;
					else
						summaries.Add(new Summary { Id = material.Id, Name = material.Name, Income = amount });
				}

			return summaries;
		}

		private static void CorrectSchematic(List<Summary> summaries, List<Factory> factories)
		{
			//===== CORRECT SCHEMATIC ======//
			bool allCorrect;
			do
			{
				allCorrect = true;
				for (int i = 0; i < summaries.Count; i++)
				{
					Summary summary = summaries[i];

					//If summary is blocked or correct
					if (summary.Blocked || Math.Floor(summary.Income * 100000) - Math.Floor(summary.Outcome * 100000) == 0) continue;
					allCorrect = false;
					double result = summary.Income - summary.Outcome;

					//Get factories, where summary as i/o. Get factories' HashCode and created array.
					int[] factoryIO;
					if (result < 0) factoryIO = factories.Where(fc => fc.Input.Count(x => x.Id == summary.Id) != 0).Select(str => str.GetHashCode()).ToArray();
					else factoryIO = factories.Where(fc => fc.Output.Count(x => x.Id == summary.Id) != 0).Select(str => str.GetHashCode()).ToArray();

					if (factoryIO.Length == 0)
					{
						summary.Income = Math.Min(summary.Income, summary.Outcome);
						summary.Outcome = Math.Min(summary.Income, summary.Outcome);
					}

					foreach (int hash in factoryIO)
					{
						Summary[] summariesIO;
						Factory factory = factories.Find(f => f.GetHashCode() == hash); //Get factory

						if (summary.Outcome == 0) summary.Outcome = summary.Income;

						double ratio = summary.Income / summary.Outcome; //Set ratio
						ratio = ratio > 1 ? 1 / ratio : ratio;

						//Get summaries, where summaries as Output.
						summariesIO = summaries.Where(sm => factory.Output.Count(fi => fi.Id == sm.Id) != 0).ToArray();
						foreach (Summary summaryIO in summariesIO)
						{
							Item item = factory.Output.First(fi => fi.Id == summaryIO.Id); //Get summary's item
							double amount = item.Amount * factory.Amount * factory.Ratio; //Caolculate amount
							summaryIO.Income -= amount - amount * ratio; //Edit Income
						}

						if (factory.Name != "Water Extractor" || result < 0)
						{
							//Get summaries, where summaries as Input.
							summariesIO = summaries.Where(sm => factory.Input.Count(fi => fi.Id == sm.Id) != 0).ToArray();
							foreach (Summary summaryIO in summariesIO)
							{
								Item item = factory.Input.First(fi => fi.Id == summaryIO.Id); //Get summary's item
								double amount = item.Amount * factory.Amount * factory.Ratio; //Caolculate amount
								summaryIO.Outcome -= amount - amount * ratio; //Edit Outcome
							}
						}

						//Reset ratio
						if (factory.Ratio == 1) factory.Ratio = ratio;
						else factory.Ratio *= ratio;
					}
				}
			}
			while (!allCorrect);

			ShowInfo(summaries, factories);
		}

		private static void ShowInfo(List<Summary> summaries, List<Factory> factories)
		{
			#region//===== SHOW RATIOS =====//
			double averageRatio = 0;

			Console.WriteLine("===== RATIOS =====");
			foreach (Factory factory in factories)
			{
				averageRatio += factory.Ratio;

				factory.Amount = Math.Ceiling(factory.Amount * factory.Ratio * 100) / 100;

				Console.WriteLine("{0, 2} ({1, 4}) {2}", Math.Ceiling(factory.Amount), factory.Amount, factory.Name.ToUpper());
			}
			Console.WriteLine("AVERAGE RATIO: {0}%", Math.Round(averageRatio / factories.Count * 10000) / 100);
			Console.WriteLine("");
			#endregion

			#region//===== SHOW FACTORIES =====//
			foreach (Factory factory in factories)
			{
				double amount;
				Material material;

				Console.WriteLine("===== {0} ({1}) =====", factory.Name.ToUpper(), factory.Amount);

				if (factory.Input != null)
					foreach (Item item in factory.Input)
					{
						if (factory.Name != "Water Extractor")
							amount = item.Amount * factory.Amount * factory.Ratio;
						else
							amount = item.Amount * factory.Amount;

						material = Material.GetMaterial(item.Id);

						Console.WriteLine("- {0,4} {1}", Math.Ceiling(amount * 100) / 100, material.Name);
					}

				if (factory.Output != null)
					foreach (Item item in factory.Output)
					{
						amount = item.Amount * factory.Amount * factory.Ratio;

						material = Material.GetMaterial(item.Id);

						Console.WriteLine("+ {0,4} {1}", Math.Floor(amount * 100) / 100, material.Name);
					}
				Console.WriteLine();
			}
			#endregion

			#region//===== SHOW SUMMARIES =====//
			if (summaries.Count != 0)
			{
				Console.WriteLine("===== SUMMARY =====");
				foreach (Summary summary in summaries)
					Console.WriteLine("{0}: {1} - {2} = {3}", summary.Name, summary.Income, summary.Outcome, summary.Income - summary.Outcome);
			}
			#endregion

			#region//===== SHOW POWER =====//
			Summary power = new Summary { Name = "Power" };
			foreach (Factory factory in factories)
			{
				Power[] powers = General.GetGeneral(factory.Id).GetPowers;

				if (powers.Length == 1)
				{
					power.Income += Convert.ToDouble(powers.First().PowerGeneration) * factory.Amount * factory.Ratio;

					if (factory.Name != "Water Extractor")
						power.Outcome += Convert.ToDouble(powers.First().PowerUse) * factory.Amount * factory.Ratio;
					else
						power.Outcome += Convert.ToDouble(powers.First().PowerUse) * factory.Amount;
				}
			}
			Console.WriteLine("{0}: {1} - {2} = {3}", power.Name, power.Income, power.Outcome, power.Income - power.Outcome);
			Console.WriteLine();
			#endregion
		}
	}

	public class Factory
	{
		public override string ToString()
		{
			double decimalAmount = Amount * Ratio;
			string input = string.Join(", ", Input.Select(sel => Material.GetMaterial(sel.Id).Name + " " + decimalAmount * sel.Amount));
			string output = string.Join(", ", Output.Select(sel => Material.GetMaterial(sel.Id).Name + " " + decimalAmount * sel.Amount));

			return $"{decimalAmount}/{Amount} {Name}: {input} => {output}";
		}
		public string Id { get; set; }
		public string Name { get; set; }
		public double Amount { get; set; }
		public Item[] Input { get; set; }
		public Item[] Output { get; set; }
		public double Ratio { get; set; }
	}

	public class Summary
	{
		public override string ToString()
		{
			return $"{Name}: {Income} - {Outcome} = {Income - Outcome} [{Blocked}]";
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public double Income { get; set; }
		public double Outcome { get; set; }
		public bool Blocked { get; set; }
		public bool Conveyor { get; set; }

	}
}