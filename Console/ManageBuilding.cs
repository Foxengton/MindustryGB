using MindustryLibrary;
using System;
using System.Linq;

namespace MindustryConsole
{
	class ManageBuilding
	{
		public static void Menu()
		{
			int offset = 2;
			int select;

			do
			{
				Console.WriteLine("╔═╤═╤═╡ BUILDING MENU ╞═════╗");
				Console.WriteLine("║0├─┤Exit                   ║");
				Console.WriteLine("║1├─┤Insert                 ║");
				Console.WriteLine("╚═╧═╧═══════════════════════╝");
				ShowAll(offset);

				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0)
					return;
				else if (select >= offset && select < General.Count + offset)
					Update(General.Generals[select - offset].Id);
				else if (select > 0)
				{
					string id = General.NextId;
					ManageGeneral.Update(new General { Id = id });
					Update(id);
				}
				else
					Formations.NotFound("Action");
			}
			while (true);
		}

		private static void Update(string id)
		{
			string[] select;

			do
			{
				General general = General.GetGeneral(id);
				if (general == null) return;

				InputOutput[] inputsOutputs = general.GetInputOutputs;
				Power[] powers = general.GetPowers;

				ShowInfo(id);

				Console.WriteLine("╔═══╤╤╡ UPDATE BUILDING ╞═════╗");
				Console.WriteLine("║ 0 ├┤ Exit                   ║");
				Console.WriteLine("║ 1 ├┤ Edit general           ║");
				Console.WriteLine("╠═══╪╪════════════════════════╝");

				//Power
				Console.WriteLine("║2 0├┤ Add Power");
				for (int i = 0; i < powers.Length; i++)
					Console.WriteLine("║2 {0}├┤ Edit {1}", i + 1, powers[i]);

				//Input/Output
				Console.WriteLine("║5 0├┤ Add Input/Output");
				for (int i = 0; i < inputsOutputs.Length; i++)
					Console.WriteLine("║5 {0}├┤ Edit {1}", i + 1, inputsOutputs[i]);

				Console.WriteLine("╠═══╪╪════════════════════════╗");
				Console.WriteLine("║ 8 ├┤ Delete                 ║");
				Console.WriteLine("╚═══╧╧════════════════════════╝");

				Console.Write("> ");
				select = Console.ReadLine().Split(' ');
				Console.Clear();

				if (select.First() == "0")
					return;
				else if (select.First() == "1")
					ManageGeneral.Update(general);
				else if (select.First() == "2")
				{
					if (select.Length == 2)
					{
						int subaction = Formations.GetInt(select[1]) - 1;

						if (subaction >= 0 && subaction < inputsOutputs.Length)
							ManagePower.Update(powers[subaction]);
						else if (subaction == -1)
							ManagePower.Update(new Power { Id = Power.NextId, GeneralId = general.Id });
						else
							Formations.NotFound("SubAction");
					}
					else
						Formations.NotFound("SubAction");
				}
				else if (select.First() == "5")
				{
					if (select.Length == 2)
					{
						int subaction = Formations.GetInt(select[1]) - 1;

						if (subaction >= 0 && subaction < inputsOutputs.Length)
							ManageInputOutput.Update(inputsOutputs[subaction]);
						else if (subaction == -1)
							ManageInputOutput.Update(new InputOutput(general.Id));
						else
							Formations.NotFound("SubAction");
					}
					else
						Formations.NotFound("SubAction");
				}
				else if (select.First() == "8")
					Delete(id);
				else Formations.NotFound("Action");
			}
			while (true);
		}
		private static void Delete(string id)
		{
			General general = General.GetGeneral(id);
			char select;

			Console.WriteLine("═════════╡ TO DELETE {0}? (Y - YES)╞═════════", general.Name.ToUpper());
			Console.Write("> ");
			select = Console.ReadKey().KeyChar;
			Console.Clear();

			if (select.ToString().ToLower() == "y")
				general.Delete();
		}

		//REFACTORING
		public static string SetItems()
		{
			int offset = 3;
			string[] input;
			double amount;
			int select;
			string items = string.Empty;
			General[] generals = General.Generals;

			do
			{
				Console.WriteLine("╔═╤═╤═╡ SET ITEMS ╞═════╗");
				Console.WriteLine("║0├─┤ Cancel            ║");
				Console.WriteLine("║1├─┤ Set Null          ║");
				Console.WriteLine("║2├─┤ Done              ║");
				Console.WriteLine("╠═╧═╧═══════════════════╝");
				ShowAll(offset); //Show items

				Console.Write("> ");
				input = Console.ReadLine().Split(' ');
				Console.Clear();

				select = Formations.GetInt(input.First());
				amount = Formations.GetDouble(input.Last());

				if (select == 0) return ""; //Cancel
				else if (select == 1) return null; //Set Null
				else if (select == 2) return items == string.Empty ? null : items; //Done
				else if (select >= offset && select < generals.Length + offset)
				{
					bool repeat; //If result have error
					do
					{
						repeat = false; //No repeat do_while
						select -= offset; //delete offset

						if (amount == 0)
						{
							Console.WriteLine("══════╡ ENTER {0} ╞══════", generals[select].Name.ToUpper());
							Console.Write("> ");
							amount = Formations.GetDouble(Console.ReadLine()); //Get amount of items
						}

						Console.Clear();

						if (amount > 0)
						{
							if (items != string.Empty) items += ";";
							items += generals[select].Id + " " + amount.ToString();
						}
						else if (amount == 0) break;
						else
						{
							Formations.NotCorrect("Amount");
							repeat = true; //Repeat do_while
						}
					}
					while (repeat);
				} //Select item
				else Formations.NotCorrect("Action"); //Error

				if (items != string.Empty) return items;
			}
			while (true);
		}

		public static void ShowAll(int offset = 0)
		{
			Console.WriteLine("┌────┬──────────────────┬──────────────┬───────┬─────┬──────────┬───────────┬───────────────┬────────────┐");
			Console.WriteLine("│ ID │ Name             │ Type         │ Power │ I/O │ Shooting │ Opt. Enh. │ Mod           │ Weight     │");
			Console.WriteLine("├────┼──────────────────┼──────────────┼───────┼─────┼──────────┼───────────┼───────────────┼────────────┤");
			for (int i = 0; i < General.Count; i++)
			{
				General gen = General.Generals[i];
				InputOutput[] inputOutput = gen.GetInputOutputs;

				int id = i + offset;
				string name = gen.Name.Length > 16 ? gen.Name.Substring(0, 13) + "..." : gen.Name.PadRight(16, ' ');
				string type = gen.GetTypeMaterial.ToString();
				string power = gen.GetPowers.Count() != 0 ? gen.GetPowers.First().PowerUse : "";

				Console.WriteLine("│ {0,2} │ {1, 16} │ {2, 12} │ {3, 5} │ {4, 3} │ {5, 8} │ {6, 9} │ {7, 13} │ {8, 10} │", id, name, type, power, inputOutput.Length, 0, 0, gen.GetMod, gen.Weight);
			}
			Console.WriteLine("└────┴──────────────────┴──────────────┴───────┴─────┴──────────┴───────────┴───────────────┴────────────┘");
		}

		private static void ShowInfo(string id)
		{
			//===== GENERAL =====//
			General general = General.GetGeneral(id);
			Console.WriteLine("══════════════╡ {0}. {1} ({2}) ╞══════════════", general.Id, general.Name, general.GetTypeMaterial);
			Console.WriteLine("──────────────────────────────────────────────────\n{0}\n──────────────────────────────────────────────────", general.Description);
			Formations.Header("GENERAL");
			Console.WriteLine(" Health: {0}", general.Health);
			Console.WriteLine(" Size: {0}", general.Size);
			Console.WriteLine(" Build Time: {0}", general.BuildTime);
			Console.WriteLine(" Build Cost: {0}", string.Join(", ", general.BuildCosts.Select(sel => sel.ToString())));
			if (general.Weight != null) Console.WriteLine(" Weight: {0}", general.Weight);

			//===== POWER =====//
			Power[] powers = general.GetPowers;

			if (powers.Length != 0)
			{
				Power powerPrev = new Power();
				string powerUse = string.Empty;
				string powerGeneration = string.Empty;
				string powerCapacity = string.Empty;

				foreach (Power power in powers)
				{
					if (power.PowerUse != null)
					{
						if (powerUse == string.Empty)
							powerUse = power.PowerUse;
						else if (power.PowerUse != powerPrev.PowerUse)
							powerUse += " / " + power.PowerUse;
					}
					if (power.PowerGeneration != null)
					{
						if (powerGeneration == string.Empty)
							powerGeneration = power.PowerGeneration;
						else if (power.PowerGeneration != powerPrev.PowerGeneration)
							powerGeneration += " / " + power.PowerGeneration;
					}
					if (power.PowerCapacity != null)
					{
						if (powerCapacity == string.Empty)
							powerCapacity = power.PowerCapacity;
						else if (power.PowerCapacity != powerPrev.PowerCapacity)
							powerCapacity += " / " + power.PowerCapacity;
					}

					powerPrev = power;
				}

				Formations.Header("POWER");
				if (powerUse != string.Empty)
					Console.WriteLine(" Power Use: {0} power units/second", powerUse);
				if (powerGeneration != string.Empty)
					Console.WriteLine(" Power Generation: {0} power units/second", powerGeneration);
				if (powerCapacity != string.Empty)
					Console.WriteLine(" Power Capacity: {0} power units", powerCapacity);
			}

			//===== INPUT/OUTPUT =====//
			InputOutput[] inputsOutputs = general.GetInputOutputs;

			if (inputsOutputs.Length != 0)
			{
				string input = string.Empty;
				string output = string.Empty;

				for (int i = 0; i < inputsOutputs.Length; i++)
				{
					if (inputsOutputs[i].Inputs != null)
					{
						if (input == string.Empty)
							input = string.Join(", ", inputsOutputs[i].Inputs.Select(sel => sel.ToString()));
						else if (inputsOutputs[i].Inputs != inputsOutputs[i - 1].Inputs)
							input += " / " + string.Join(", ", inputsOutputs[i].Inputs.Select(sel => sel.ToString()));
					}
					if (inputsOutputs[i].Outputs != null)
					{
						if (output == string.Empty)
							output = string.Join(", ", inputsOutputs[i].Outputs.Select(sel => sel.ToString()));
						else if (inputsOutputs[i].Outputs != inputsOutputs[i - 1].Outputs)
							output += " / " + string.Join(", ", inputsOutputs[i].Outputs.Select(sel => sel.ToString()));
					}
				}

				Formations.Header("INPUT/OUTPUT");
				if (input != string.Empty)
					Console.WriteLine(" Input: {0}", input);
				if (output != string.Empty)
					Console.WriteLine(" Output: {0}", output);
				Console.WriteLine(" Production Time: {0} second", inputsOutputs[0].ProductionTime);
			}
		}
	}
}