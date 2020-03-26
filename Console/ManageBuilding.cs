using System;
using System.Linq;
using MindustryLibrary;

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
				Console.WriteLine("╔════════╡ MENU ╞════════╗");
				Console.WriteLine("╟─┐ ┌────────────────────╢");
				Console.WriteLine("║0├─┤Exit                ║");
				Console.WriteLine("║1├─┤Insert              ║");
				Console.WriteLine("╟─┘ └────────────────────╢");
				Console.WriteLine("╚════════════════════════╝");
				ShowAll(General.generals, offset);
				Console.Write("> ");
				
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select >= offset && select < General.generals.Length + offset)
					Select(select - offset);
				else if (select > 0)
					ManageGeneral.Update(new General { Id = General.NextId });
				else if (select != 0)
					Formations.NotFound("Action");
			}
			while (select != 0);
		}

		private static void Select(int id)
		{
			string[] select;

			do
			{
				General general = General.generals[id];
				InputOutput[] inputsOutputs = InputOutput.inputsOutputs.Where(io => io.GeneralId == general.Id).ToArray();

				ShowInfo(general, inputsOutputs);

				Console.WriteLine("╔════════╡ SELECT ╞═══════╗");
				Console.WriteLine("╟───┐ ┌───────────────────╢");
				Console.WriteLine("║ 0 ├─┤ Exit              ║");

				//General
				if (general != null)
					Console.WriteLine("║ 1 ├─┤ Edit general      ║");
				else
					Console.WriteLine("║ 1 ├─┤ Add general       ║");

				//Input/Output
				Console.WriteLine("║5 0├─┤ Add Input/Output  ║");
				for (int i = 0; i < inputsOutputs.Length; i++)
					Console.WriteLine("║5 {0}├─┤ Edit Input/Output ║", i + 1);

				Console.WriteLine("║ 9 ├─┤ Delete            ║");
				Console.WriteLine("╟───┘ └───────────────────╢");
				Console.WriteLine("╚═════════════════════════╝");
				Console.Write("> ");
				select = Console.ReadLine().Split(' ');
				Console.Clear();

				switch (select[0])
				{
					case "0": break;
					case "1": ManageGeneral.Update(general); break;
					case "5":
						{
							if (select.Length == 2)
							{
								int subaction = Formations.GetInt(select[1]) - 1;

								if (subaction >= 0 && subaction < inputsOutputs.Length)
									ManageInputOutput.Update(inputsOutputs[subaction]);
								else if(subaction == -1)
									ManageInputOutput.Update(new InputOutput { Id = InputOutput.NextId, GeneralId = general.Id });
								else
									Formations.NotFound("SubAction");
							}
							else
								Formations.NotFound("Action");
							break;
						}
					case "9": general.Delete(); select[0] = "0"; break;
					default: Formations.NotFound("Action"); break;
				}
			}
			while (select[0] != "0");
		}

		private static void ShowAll(General[] generals, int offset = 0)
		{
			Console.WriteLine("┌────┬──────────────────┬───────┬─────────┬───────┬──────────────┬──────────┬───────────────────────┬────────────┐");
			Console.WriteLine("│ ID │ Name             │ Power │ Liquids │ Items │ Input/Output │ Shooting │ Optional Enhancements │ Weight     │");
			Console.WriteLine("├────┼──────────────────┼───────┼─────────┼───────┼──────────────┼──────────┼───────────────────────┼────────────┤");
			for (int i = 0; i < generals.Length; i++)
			{
				General gen = generals[i];
				Console.WriteLine("│ {0,2} │ {1, 16} │ {2, 5} │ {3, 7} │ {4, 5} │ {5, 12} │ {6, 8} │ {7, 21} │ {8, 10} │", i + offset, gen.Name.Length > 16 ? gen.Name.Substring(0, 13) + "..." : gen.Name.PadRight(16, ' '), 0, 0, 0, InputOutput.inputsOutputs.Where(io => io.GeneralId == gen.Id).ToArray().Length, 0, 0, gen.Weight);
			}
			Console.WriteLine("└────┴──────────────────┴───────┴─────────┴───────┴──────────────┴──────────┴───────────────────────┴────────────┘");
		}

		private static void ShowInfo(General general, InputOutput[] inputsOutputs)
		{
			Console.WriteLine("══════════════╡ INFO ╞══════════════");
			if (general.Id != null) Console.Write("[{0}]", general.Id);
			if (general.Name != null) Console.Write(" {0}", general.Name);
			if (general.Type != null) Console.Write(" ({0})", general.Type);
			Console.WriteLine();
			if (general.Description != null) Console.WriteLine("──────────────────────────────────────────────────\n{0}\n──────────────────────────────────────────────────", general.Description);

			Formations.Header("GENERAL");
			if (general.Health != null) Console.WriteLine(" Health: {0}", general.Health);
			if (general.Size != null) Console.WriteLine(" Size: {0}", general.Size);
			if (general.BuildTime != null) Console.WriteLine(" Build Time: {0}", general.BuildTime);
			if (general.BuildCost != null) Console.WriteLine(" Build Cost: {0}", ManageMaterial.NormalizateItems(general.BuildCost.Split(';')));
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
							input = ManageMaterial.NormalizateItems(inputsOutputs[i].Input.Split(';'));
						else if (inputsOutputs[i].Input != inputsOutputs[i - 1].Input)
							input += " / " + ManageMaterial.NormalizateItems(inputsOutputs[i].Input.Split(';'));
					}
					if (inputsOutputs[i].Output != null)
					{
						if (output == string.Empty)
							output = ManageMaterial.NormalizateItems(inputsOutputs[i].Output.Split(';'));
						else if (inputsOutputs[i].Output != inputsOutputs[i - 1].Output)
							output += "/ " + ManageMaterial.NormalizateItems(inputsOutputs[i].Output.Split(';'));
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
