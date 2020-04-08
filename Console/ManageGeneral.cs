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
				Console.WriteLine("[2] Type: {0}", general.Type);
				Console.WriteLine("[3] Description: {0}", general.Description == null ? "null" : general.Description);
				Console.WriteLine("[4] Health: {0}", general.Health);
				Console.WriteLine("[5] Size: {0}", general.Size);
				Console.WriteLine("[6] Build Time: {0}", general.BuildTime);
				Console.WriteLine("[7] Build Cost: {0}", general.BuildCost == null ? "null" : string.Join(", ", general.BuildCosts.Select(sel => sel.ToString())));
				Console.WriteLine("[8] Mod: {0}", general.Mod);
				Console.WriteLine("Weight: {0}", general.Weight);
				Console.WriteLine("[9] Save");
				Console.WriteLine("[0] Exit");

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0') return;
				else if (select == '1')
				{
					string name = Formations.SetValue("name", "string");

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
				else if (select == '2') general.Type = Formations.SetValue("type", "string");
				else if (select == '3') general.Description = Formations.SetValue("description", "string");
				else if (select == '4') general.Health = Formations.SetValue("health", "int");
				else if (select == '5') general.Size = Formations.SetValue("size", "size");
				else if (select == '6') general.BuildTime = Formations.SetValue("build Time", "double");
				else if (select == '7')
				{
					Item[] buildCost = ManageMaterial.SetItems();
					if (buildCost != null) general.BuildCost = string.Join(";", buildCost.Select(inp => inp.Id + " " + inp.Amount));
				}
				else if (select == '8') general.Mod = Formations.SetValue("mod", "string");
				else if (select == '9')
				{
					general.Save();
					return;
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}
	}
}