using MindustryLibrary;
using System;
using System.Linq;

namespace MindustryConsole
{
	class ManageGeneral
	{
		public static void Update(General general)
		{
			char select;
			do
			{
				Console.WriteLine("═════════╡ GENERAL ╞═════════");
				Console.WriteLine("ID: {0}", general.Id);
				Console.WriteLine("[1] Name: {0}", general.Name);
				Console.WriteLine("[2] Type: {0}", general.GetTypeMaterial);
				Console.WriteLine("[3] Description: {0}", general.Description);
				Console.WriteLine("[4] Health: {0}", general.Health);
				Console.WriteLine("[5] Size: {0}", general.Size);
				Console.WriteLine("[6] Build Time: {0}", general.BuildTime);
				Console.WriteLine("[7] Build Cost: {0}", general.BuildCosts);
				Console.WriteLine("[8] Mod: {0}", general.GetMod);
				Console.WriteLine("Weight: {0}", general.Weight);
				Console.WriteLine("[9] Save");
				Console.WriteLine("[0] Exit");

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0')
				{
					bool allCorrect = true;

					General targetGeneral = General.GetGeneral(general.Id);

					if (targetGeneral != null && general.Name != targetGeneral.Name || general.Type != targetGeneral.Type || general.BuildCosts != targetGeneral.BuildCosts || general.Mod != targetGeneral.Mod)
						allCorrect = false;
					else if (general.Name != null || general.Type != null || general.BuildCosts != null || general.Mod != null)
						allCorrect = false;

					if (allCorrect)
						return;
					else
					{
						Console.WriteLine("═════════╡ YOU ARE SURE WANT TO EXIT? (Y - YES)╞═════════");
						Console.Write("> ");
						select = Console.ReadKey().KeyChar;
						Console.Clear();

						if (select.ToString().ToLower() == "y")
							return;
					}
				}
				else if (select == '1')
				{
					string name = Formations.GetValue("name", "string");
					if (name == null)
					{
						Formations.NotCorrect("Name");
						continue;
					}

					if (General.Generals.Count(gen => gen.Name == name) != 0)
					{
						Console.WriteLine("═════════╡ THIS BUILDING ALREADY EXIST ╞═════════");
						Console.WriteLine("1. Go to edit");
						Console.WriteLine("0. Exit");

						Console.Write("> ");
						select = Console.ReadKey().KeyChar;
						Console.Clear();

						if (select == '1')
						{
							Update(General.Generals.Where(gen => gen.Name == name).First());
							return;
						}
					}

					general.Name = name;
				}
				else if (select == '2')
				{
					string type = ManageType.SetType("Block");
					if (type != null)
						general.Type = type;
				}
				else if (select == '3') general.Description = Formations.GetValue("description", "string");
				else if (select == '4') general.Health = Formations.GetValue("health", "int");
				else if (select == '5') general.Size = Formations.GetValue("size", "size");
				else if (select == '6') general.BuildTime = Formations.GetValue("build Time", "double");
				else if (select == '7')
				{
					Item[] buildCost = ManageMaterial.SetItems();
					if (buildCost != null)
						general.BuildCost = string.Join(";", buildCost.Select(inp => inp.Id + " " + inp.Amount));
				}
				else if (select == '8')
				{
					string mod = ManageMod.SetMod();
					if (mod != null)
						general.Mod = mod;
				}
				else if (select == '9')
				{
					bool allCorrect = true;

					if (general.Name == null)
					{
						allCorrect = false;
						Formations.NotCorrect("Name");
					}
					if (general.Type == null)
					{
						allCorrect = false;
						Formations.NotCorrect("Type");
					}
					if (general.BuildCosts == null)
					{
						allCorrect = false;
						Formations.NotCorrect("Build Cost");
					}
					if (general.Mod == null)
					{
						allCorrect = false;
						Formations.NotCorrect("Mod");
					}

					if (allCorrect)
					{
						general.Save();
						return;
					}
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}
	}
}