using MindustryLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
				Console.WriteLine("╔═════╡ MENU ╞═════╗");
				Console.WriteLine("╟─┐ ┌──────────────╢");
				Console.WriteLine("║0├─┤ Exit         ║");
				Console.WriteLine("║1├─┤ Insert       ║");
				Console.WriteLine("║2├─┤ Reset        ║");
				Console.WriteLine("╟─┘ └──────────────╢");
				Console.WriteLine("╚══════════════════╝");
				ShowAll(Material.materials, false, offset);
				Console.Write("> ");

				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select >= offset && select < Material.materials.Length + offset)
					Update(Material.materials[select - offset]);
				else if (select == 2)
					Material.Reset();
				else if (select > 0)
					Update(new Material { Id = Material.NextId });
				else if (select != 0) Formations.NotFound("Action");
			}
			while (select != 0);
		}

		public static Material Select(bool onlyAvailable = false)
		{
			int offset = 1;
			int select;

			Console.WriteLine("╔═════╡ SELECT ╞═════╗");
			Console.WriteLine("╟─┐ ┌────────────────╢");
			Console.WriteLine("║0├─┤ Exit           ║");
			Console.WriteLine("╟─┘ └────────────────╢");
			Console.WriteLine("╚════════════════════╝");
			ShowAll(Material.materials, onlyAvailable, offset);
			Console.Write("> ");
			select = Formations.GetInt(Console.ReadLine());
			Console.Clear();

			if (select >= offset) return Material.materials[select - offset];

			return null;
		}

		private static void Update(Material material)
		{
			char select;
			do
			{
				Console.Clear();
				Console.WriteLine("═════════╡ MATERIAL ╞═════════");
				Console.WriteLine("ID: {0}", material.Id);
				Console.WriteLine("[1] Name: {0}", material.Name);
				Console.WriteLine("[2] Type: {0}", material.Type);
				Console.WriteLine("[3] Mod: {0}", material.Mod);
				Console.WriteLine("Weight: {0}", material.Weight);
				Console.WriteLine("[4] Color: {0}", material.Color);
				Console.WriteLine("[9] Save");
				Console.WriteLine("[0] Exit");

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;

				switch (select)
				{
					case '0': break;
					case '1': material.Name = Formations.SetValue("Name", "string"); break;
					case '2': material.Type = Formations.SetValue("Type", "string"); break;
					case '3': material.Mod = Formations.SetValue("Mod", "string"); break;
					case '4': material.Color = Formations.SetValue("color", "string"); break;
					case '9': material.Save(); break;
					default: Formations.NotFound("Action"); break;
				}
			}
			while (select != '0');
		}

		public static string SetItems(bool OnlyAvailable = true, bool onlyOne = false)
		{
			int offset = 3;
			int select;
			string[] items = new string[Material.materials.Length];

			do
			{
				Console.WriteLine("╔═════╡ MENU ╞═════╗");
				Console.WriteLine("╟─┐ ┌──────────────╢");
				Console.WriteLine("║0├─┤ Cancel       ║");
				Console.WriteLine("║1├─┤ Set Null     ║");
				Console.WriteLine("║2├─┤ Done         ║");
				Console.WriteLine("╟─┘ └──────────────╢");
				Console.WriteLine("╚══════════════════╝");
				ShowAll(Material.materials, OnlyAvailable, offset); //Show all items
				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0) return ""; //Cancel
				else if (select == 1) return null; //Set Null
				else if (select == 2) return string.Join(";", items); //Done
				else if (select >= offset && select < Material.materials.Length + offset)
				{
					bool repeat; //If result have error

					do
					{
						repeat = false; //No repeat do_while
						select -= offset; //delete offset

						Console.WriteLine("══════╡ ENTER {0} ╞══════", Material.materials[select].Name.ToUpper());
						Console.Write("> ");
						double amount = Formations.GetDouble(Console.ReadLine()); //Get amount of items
						Console.Clear();

						if (amount > 0) items[select] = amount.ToString();
						else if (amount == 0) break;
						else
						{
							Formations.NotCorrect("Amount");
							repeat = true; //Repeat do_while
						}
					}
					while (repeat);
				} //Select item
				else Formations.NotCorrect("Amount"); //Error
			}
			while (true);
		}

		public static string NormalizateItems(string[] items)
		{
			string result = string.Empty;

			for (int i = 0; i < Material.materials.Length; i++)
				if (items[i] != "") result += $"{items[i]} {Material.materials[i].Name} ";

			return result;
		}

		private static void ShowAll(Material[] materials, bool onlyAvailable = false, int offset = 0)
		{
			Console.WriteLine("┌────┬──────────────────────┬─────────┬────────────┐");
			Console.WriteLine("│ ID │ Name                 │ Type    │ Weight     │");
			Console.WriteLine("├────┼──────────────────────┼─────────┼────────────┤");

			for (int i = 0; i < materials.Length; i++)
			{
				Material mat = materials[i];

				if (onlyAvailable == true && mat.Weight != null)
					Console.WriteLine("│ {0,2} │ {1, 20} │ {2, 7} │ {3, 10} │", i + offset, mat.Name.PadRight(20, ' '), mat.Type, mat.Weight);
				else if (onlyAvailable == false)
					Console.WriteLine("│ {0,2} │ {1, 20} │ {2, 7} │ {3, 10} │", i + offset, mat.Name.PadRight(20, ' '), mat.Type, mat.Weight);
			}
			Console.WriteLine("└────┴──────────────────────┴─────────┴────────────┘");
		}
	}
}
