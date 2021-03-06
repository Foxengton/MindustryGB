﻿using MindustryLibrary;
using System;
using System.Collections.Generic;
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
				else if (select == 2) Material.ResetAll();
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
				Console.WriteLine("║2├─┤ Type: {0}", material.GetTypeMaterial);
				Console.WriteLine("║3├─┤ Mod: {0}", material.GetMod);
				Console.WriteLine("║4├─┤ Color: {0}", material.Color);
				
				string ProducedIn = string.Join(", ", General.Generals.Where(gen => InputOutput.InputsOutputs.Count(io => io.Outputs != null && io.GeneralId == gen.Id && io.Outputs.Count(it => it.Id == material.Id) != 0) != 0).Select(sel => sel.Name));
				Console.WriteLine("║ ├─┤ Produced in: {0}", ProducedIn);

				string RequiredFor = string.Join(", ", General.Generals.Where(gen => InputOutput.InputsOutputs.Count(io => io.Inputs != null && io.GeneralId == gen.Id && io.Inputs.Count(it => it.Id == material.Id) != 0) != 0).Select(sel => sel.Name));
				Console.WriteLine("║ ├─┤ Required for: {0}", RequiredFor);
				
				string UsedToBuild = string.Join(", ", General.Generals.Where(gen => gen.BuildCosts.Count(item => item.Id == material.Id) != 0).Select(sel => sel.Name));
				Console.WriteLine("║ ├─┤ Used to build: {0}", UsedToBuild);
				Console.WriteLine("║8├─┤ Delete");
				Console.WriteLine("║9├─┤ Save");
				Console.WriteLine("╚═╧═╧═════════════════{0}", "═".PadRight(material.Id.Length + material.Name.Length, '═'));

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0') return;
				else if (select == '1') material.Name = Formations.GetValue("Name", "string");
				else if (select == '2')
				{
					string type = ManageType.SetType("Item");
					if (type != null)
						material.Type = type;
				}
				else if (select == '3')
				{
					string mod = ManageMod.SetMod();
					if (mod != null)
						material.Mod = mod;
				}
				else if (select == '4') material.Color = Formations.GetValue("Color", "string");
				else if (select == '8')
				{
					Console.WriteLine("═════════╡ TO DELETE {0}? (Y - YES)╞═════════", material.Name.ToUpper());
					Console.Write("> ");
					select = Console.ReadKey().KeyChar;
					Console.Clear();

					if (select.ToString().ToLower() == "y")
					{
						material.Delete();
						return;
					}
				}
				else if (select == '9') { material.Save(); return; }
				else Formations.NotFound("Action");
			}
			while (true);
		}

		public static Item[] SetItems(bool onlyOne = false)
		{
			int offset = 3;
			string[] input;
			double amount;
			int select;
			List<Item> items = new List<Item>();
			Material[] materials = Material.Materials;

			do
			{
				Console.WriteLine("╔═╤═╤═╡ SET ITEMS ╞═════╗");
				Console.WriteLine("║0├─┤ Cancel            ║");
				Console.WriteLine("║1├─┤ Set Null          ║");
				Console.WriteLine("║2├─┤ Done              ║");
				Console.WriteLine("╠═╧═╧═══════════════════╝");
				Console.WriteLine("║Result: {0}", string.Join(", ", items)); //show result
				Console.WriteLine("╚════════════════════════");
				ShowAll(offset, materials); //Show items

				Console.Write("> ");
				input = Console.ReadLine().Split(' ');
				Console.Clear();

				select = Formations.GetInt(input.First());

				if (input.Length == 2)
					amount = Formations.GetDouble(input.Last());
				else amount = 0;

				if (select == 0) return null; //Cancel
				else if (select == 1) return null; //Set Null
				else if (select == 2) return items.ToArray(); //Done
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
							Console.Clear();
						}

						if (amount > 0) items.Add(new Item { Id = materials[select].Id, Amount = amount });
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

				if (onlyOne && items != null) return items.ToArray();
			}
			while (true);
		}

		private static void ShowAll(int offset = 0, Material[] materials = null)
		{
			if (materials == null) materials = Material.Materials;

			Console.WriteLine("┌────┬──────────────────────┬─────────┬───────────────┬────────────┬─────────┐");
			Console.WriteLine("│ ID │ Name                 │ Type    │ Mod           │ Weight     │ Color   │");
			Console.WriteLine("├────┼──────────────────────┼─────────┼───────────────┼────────────┼─────────┤");

			for (int i = 0; i < materials.Length; i++)
			{
				Material mat = materials[i];
				int id = i + offset;
				string name = mat.Name.PadRight(20, ' ');

				Console.WriteLine("│ {0,2} │ {1, 20} │ {2, 7} │ {3, 13} │ {4, 10} │ {5, 7} │", id, name, mat.GetTypeMaterial, mat.GetMod, mat.Weight, mat.Color);
			}
			Console.WriteLine("└────┴──────────────────────┴─────────┴───────────────┴────────────┴─────────┘");
		}
	}
}