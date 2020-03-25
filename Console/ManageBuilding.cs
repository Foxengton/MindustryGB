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
					Select(General.generals[select - offset].Id);
				else if (select > 0)
					ManageGeneral.Update(new General { Id = General.NextId });
				else if (select != 0) Formations.NotFound("Action");
			}
			while (select != 0);
		}

		private static void Select(string id)
		{
			General[] generals = General.generals.Where(gen => gen.Id == id).ToArray();
			InputOutput[] inputsOutputs = InputOutput.inputsOutputs.Where(io => io.GeneralId == id).ToArray();
			int totalWidth = 17;
			string[] select;

			do
			{
				ShowAll(generals);

				Console.WriteLine("╔════════╡ SELECT ╞═══════╗");
				Console.WriteLine("╟───┐ ┌─{0,17}─╢", "─".PadLeft(totalWidth, '─'));
				Console.WriteLine("║0  ├─┤ {0,17} ║", "Exit".PadRight(totalWidth, ' '));

				//General
				if (generals.Length != 0) Console.WriteLine("║1  ├─┤ {0,17} ║", "Edit general".PadRight(totalWidth, ' '));
				else Console.WriteLine("║1  ├─┤ {0,17} ║", "Add general".PadRight(totalWidth, ' '));

				//Input/Output
				Console.WriteLine("║5 0├─┤ {0,17} ║", "Add Input/Output".PadRight(totalWidth, ' '));
				for (int i = 0; i < inputsOutputs.Length; i++)
					Console.WriteLine("║5 {1}├─┤ {0,17} ║", "Edit Input/Output".PadRight(totalWidth, ' '), i + 1);

				Console.WriteLine("╟───┘ └─{0,17}─╢", "─".PadLeft(totalWidth, '─'));
				Console.WriteLine("╚═════════════════════════╝");
				Console.Write("> ");
				select = Console.ReadLine().Split(' ');
				Console.Clear();

				switch (select[0])
				{
					case "0": break;
					case "1": ManageGeneral.Update(generals[0]); break;
					case "5":
						{
							if (select.Length == 2)
							{
								int subaction = Formations.GetInt(select[1]) - 1;

								if (subaction > 0 && subaction < inputsOutputs.Length)
									ManageInputOutput.Update(inputsOutputs[subaction]);
								else if(subaction == -1)
									ManageInputOutput.Update(new InputOutput { Id = InputOutput.NextId, GeneralId = id });
								else
									Formations.NotFound("SubAction");
							}
							else
								Formations.NotFound("Action");
							break;
						}
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

		private static void ShowInfo(General general)
		{
			Console.Clear();
			Console.WriteLine("═════════╡ INFO ╞═════════");

			if (general.Id != null) Console.Write("[{0}]", general.Id);
			if (general.Name != null) Console.Write(" {0}", general.Name);
			if (general.Type != null) Console.Write(" ({0})", general.Type);
			Console.WriteLine();
			if (general.Description != null) Console.WriteLine("──────────────────────────────────────────────────\n{0}\n──────────────────────────────────────────────────", general.Description);

			Formations.Header("────┤ GENERAL ├────");
			if (general.Health != null) Console.WriteLine("\tHealth: {0}", general.Health);
			if (general.Size != null) Console.WriteLine("\tSize: {0}", general.Size);
			if (general.BuildTime != null) Console.WriteLine("\tBuild Time: {0}", general.BuildTime);
			//if (general.buildCost != null) Console.WriteLine("\tBuild Cost: {0}", NormalizateItems(general.buildCost.Split(';')));
			if (general.Weight != null) Console.WriteLine("\tWeight: {0}", general.Weight);
		}
	}
}
