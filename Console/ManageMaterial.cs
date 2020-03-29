using MindustryLibrary;
using System;
using System.Linq;

namespace MindustryConsole
{
	class ManageMaterial
	{
		public static void Menu()
		{
			int offset = 3;
			int select;

			do
			{
				Console.WriteLine("╔═╤═╤═╡ MATERIAL MENU ╞═════╗");
				Console.WriteLine("║0├─┤ Exit                  ║");
				Console.WriteLine("║1├─┤ Insert                ║");
				Console.WriteLine("║2├─┤ Reset                 ║");
				Console.WriteLine("╚═╧═╧═══════════════════════╝");
				ShowAll(offset);

				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0) return;
				else if (select == 2) Material.Reset();
				else if (select >= offset && select < Material.Count + offset) Select(Material.Materials[select - offset]);
				else if (select > 0) Select(new Material { Id = Material.NextId });
				else Formations.NotFound("Action");
			}
			while (true);
		}

		private static void Select(Material material)
		{
			char select;

			do
			{
				Console.WriteLine("╔═╤═╤═══╡ {0}. {1} ╞════════", material.Id, material.Name.ToUpper());
				Console.WriteLine("║0├─┤ Exit");
				Console.WriteLine("║1├─┤ Change name");
				Console.WriteLine("║2├─┤ Type: {0}", material.Type);
				Console.WriteLine("║3├─┤ Mod: {0}", material.Mod);
				Console.WriteLine("║4├─┤ Color: {0}", material.Color);
				Console.WriteLine("║9├─┤ Save");
				Console.WriteLine("╚═╧═╧═════════════════{0}", "═".PadRight(material.Id.Length + material.Name.Length, '═'));

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0') return;
				else if (select == '1') material.Name = Formations.SetValue("Name", "string");
				else if (select == '2') material.Type = Formations.SetValue("Type", "string");
				else if (select == '3') material.Mod = Formations.SetValue("Mod", "string");
				else if (select == '4') material.Color = Formations.SetValue("color", "string");
				else if (select == '9') { material.Save(); return; }
				else Formations.NotFound("Action");
			}
			while (true);
		}

		public static string SetItems(bool OnlyAvailable = true, bool onlyOne = false)
		{
			int offset = 3;
			string[] input;
			double amount;
			int select;
			string items = string.Empty;
			Material[] materials;

			if (OnlyAvailable) materials = Material.GetAvailable();
			else materials = Material.Materials;

			do
			{
				amount = 0;

				Console.WriteLine("╔═╤═╤═╡ SET ITEMS ╞═════╗");
				Console.WriteLine("║0├─┤ Cancel            ║");
				Console.WriteLine("║1├─┤ Set Null          ║");
				Console.WriteLine("║2├─┤ Done              ║");
				Console.WriteLine("╠═╧═╧═══════════════════╝");
				Console.WriteLine("║Result: {0}", NormalizateItems(items)); //show result
				Console.WriteLine("╚═══════{0}", "═".PadRight(items.Length, '═'));
				ShowAll(offset, materials); //Show items

				Console.Write("> ");
				input = Console.ReadLine().Split(' ');
				Console.Clear();

				select = Formations.GetInt(input.First());
				amount = Formations.GetDouble(input.Last());

				if (select == 0) return ""; //Cancel
				else if (select == 1) return null; //Set Null
				else if (select == 2) return items == string.Empty ? null : items; //Done
				else if (select >= offset && select < materials.Length + offset)
				{
					bool repeat; //If result have error
					do
					{
						repeat = false; //No repeat do_while
						select -= offset; //delete offset

						if (amount == 0)
						{
							Console.WriteLine("══════╡ ENTER {0} ╞══════", materials[select].Name.ToUpper());
							Console.Write("> ");
							amount = Formations.GetDouble(Console.ReadLine()); //Get amount of items
						}
						
						Console.Clear();

						if (amount > 0)
						{
							if (items != string.Empty) items += ";";
							items += materials[select].Id + " " + amount.ToString();
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

				if (onlyOne && items != string.Empty) return items;
			}
			while (true);
		}

		public static int GetIndex(Material[] materials)
		{
			int offset = 1;
			int select;

			do
			{
				Console.WriteLine("╔═╤═╤═╡ GET INDEX ╞═════╗");
				Console.WriteLine("║0├─┤ Cancel            ║");
				Console.WriteLine("╚═╧═╧═══════════════════╝");
				ShowAll(offset, materials); //Show all items

				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0) return -1; //Cancel
				else if (select >= offset && select < materials.Length + offset) return select - offset;
				else Formations.NotCorrect("Amount"); //Error
			}
			while (true);
		}

		public static string NormalizateItems(string item)
		{
			string result = string.Empty;

			if (item != null && item != "")
			{
				string[] items = item.Split(';');

				for (int i = 0; i < items.Length; i++)
				{
					if (items[i] == "") continue;

					if (result != string.Empty) result += ", ";
					result += $"{items[i].Split(' ').Last()} {Material.GetMaterial(items[i].Split(' ').First()).Name}";
				}
			}

			return result;
		}

		private static void ShowAll(int offset = 0, Material[] materials = null)
		{
			if (materials == null) materials = Material.Materials;

			Console.WriteLine("┌────┬──────────────────────┬─────────┬────────────┬────────────┬─────────┐");
			Console.WriteLine("│ ID │ Name                 │ Type    │ Mod        │ Weight     │ Color   │");
			Console.WriteLine("├────┼──────────────────────┼─────────┼────────────┼────────────┼─────────┤");

			for (int i = 0; i < materials.Length; i++)
			{
				Material mat = materials[i];
				int id = i + offset;
				string name = mat.Name.PadRight(20, ' ');

				Console.WriteLine("│ {0,2} │ {1, 20} │ {2, 7} │ {3, 10} │ {4, 10} │ {5, 7} │", id, name, mat.Type, mat.Mod, mat.Weight, mat.Color);
			}
			Console.WriteLine("└────┴──────────────────────┴─────────┴────────────┴────────────┴─────────┘");
		}
	}
}