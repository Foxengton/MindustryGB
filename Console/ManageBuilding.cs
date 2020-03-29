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

				if (select == 0) return;
				else if (select >= offset && select < General.Generals.Length + offset) Update(General.Generals[select - offset].Id);
				else if (select > 0)
				{
					string id = General.NextId;
					ManageGeneral.Update(new General { Id = id });
					Update(id);
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}

		private static void Update(string id)
		{
			string[] select;

			do
			{
				General general = General.GetGeneral(id);
				InputOutput[] inputsOutputs = InputOutput.InputsOutputs.Where(io => io.GeneralId == general.Id).ToArray();

				ShowInfo(general, inputsOutputs);

				Console.WriteLine("╔════════╡ UPDATE ╞═══════╗");
				Console.WriteLine("╟───┐ ┌───────────────────╢");
				Console.WriteLine("║ 0 ├─┤ Exit              ║");

				//General
				if (general != null) Console.WriteLine("║ 1 ├─┤ Edit general      ║");
				else Console.WriteLine("║ 1 ├─┤ Add general       ║");

				//Input/Output
				Console.WriteLine("║5 0├─┤ Add Input/Output  ║");
				for (int i = 0; i < inputsOutputs.Length; i++)
					Console.WriteLine("║5 {0}├─┤ Edit {1}/{2} ║", i + 1, ManageMaterial.NormalizateItems(inputsOutputs[i].Input), ManageMaterial.NormalizateItems(inputsOutputs[i].Output));

				Console.WriteLine("║ 9 ├─┤ Delete            ║");
				Console.WriteLine("╟───┘ └───────────────────╢");
				Console.WriteLine("╚═════════════════════════╝");

				Console.Write("> ");
				select = Console.ReadLine().Split(' ');
				Console.Clear();

				if (select[0] == "0") return;
				else if (select[0] == "1") ManageGeneral.Update(general);
				else if (select[0] == "5")
				{
					if (select.Length == 2)
					{
						int subaction = Formations.GetInt(select[1]) - 1;

						if (subaction >= 0 && subaction < inputsOutputs.Length) ManageInputOutput.Update(inputsOutputs[subaction]);
						else if (subaction == -1) ManageInputOutput.Update(new InputOutput { Id = InputOutput.NextId, GeneralId = general.Id });
						else Formations.NotFound("SubAction");
					}
					else Formations.NotFound("SubAction");
				}
				else if (select[0] == "9")
				{
					general.Delete();
					return;
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}

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
				amount = 0;

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
			Console.WriteLine("┌────┬──────────────────┬───────┬─────────┬───────┬─────┬──────────┬───────────┬────────────┬────────────┐");
			Console.WriteLine("│ ID │ Name             │ Power │ Liquids │ Items │ I/O │ Shooting │ Opt. Enh. │ Mod        │ Weight     │");
			Console.WriteLine("├────┼──────────────────┼───────┼─────────┼───────┼─────┼──────────┼───────────┼────────────┼────────────┤");
			for (int i = 0; i < General.Count; i++)
			{
				General gen = General.Generals[i];
				InputOutput[] inputOutput = InputOutput.GetGeneral(gen.Id);

				int id = i + offset;
				string name = gen.Name.Length > 16 ? gen.Name.Substring(0, 13) + "..." : gen.Name.PadRight(16, ' ');
				string power = "0";
				string liquids = "0";
				string items = "0";


				Console.WriteLine("│ {0,2} │ {1, 16} │ {2, 5} │ {3, 7} │ {4, 5} │ {5, 3} │ {6, 8} │ {7, 9} │ {8, 10} │ {9, 10} │", id, name, power, liquids, items, inputOutput.Length, 0, 0, gen.Mod, gen.Weight);
			}
			Console.WriteLine("└────┴──────────────────┴───────┴─────────┴───────┴─────┴──────────┴───────────┴────────────┴────────────┘");
		}

		private static void ShowInfo(General general, InputOutput[] inputsOutputs)
		{
			Console.Write("══════════════╡ ");
			if (general.Id != null) Console.Write("{0}.", general.Id);
			if (general.Name != null) Console.Write(" {0}", general.Name);
			if (general.Type != null) Console.Write(" ({0})", general.Type);
			Console.WriteLine(" ╞══════════════");
			if (general.Description != null) Console.WriteLine("──────────────────────────────────────────────────\n{0}\n──────────────────────────────────────────────────", general.Description);

			Formations.Header("GENERAL");
			if (general.Health != null) Console.WriteLine(" Health: {0}", general.Health);
			if (general.Size != null) Console.WriteLine(" Size: {0}", general.Size);
			if (general.BuildTime != null) Console.WriteLine(" Build Time: {0}", general.BuildTime);
			if (general.BuildCost != null) Console.WriteLine(" Build Cost: {0}", ManageMaterial.NormalizateItems(general.BuildCost));
			if (general.Weight != null) Console.WriteLine(" Weight: {0}", general.Weight);

			if (inputsOutputs.Length != 0)
			{
				string input = string.Empty;
				string output = string.Empty;

				for (int i = 0; i < inputsOutputs.Length; i++)
				{
					if (inputsOutputs[i].Input != null)
					{
						if (input == string.Empty)
							input = ManageMaterial.NormalizateItems(inputsOutputs[i].Input);
						else if (inputsOutputs[i].Input != inputsOutputs[i - 1].Input)
							input += " / " + ManageMaterial.NormalizateItems(inputsOutputs[i].Input);
					}
					if (inputsOutputs[i].Output != null)
					{
						if (output == string.Empty)
							output = ManageMaterial.NormalizateItems(inputsOutputs[i].Output);
						else if (inputsOutputs[i].Output != inputsOutputs[i - 1].Output)
							output += "/ " + ManageMaterial.NormalizateItems(inputsOutputs[i].Output);
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