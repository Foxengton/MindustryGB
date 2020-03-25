using MindustryLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindustryConsole
{
	public class ManageInputOutput
	{
		public static void Update(InputOutput inputOutput)
		{
			char select;
			do
			{
				Console.WriteLine("═════════╡ INPUT/OUTPUT ╞═════════");
				Console.WriteLine("ID: {0}", inputOutput.Id);
				Console.WriteLine("Owner: {0}", General.generals.Where(gen => gen.Id == inputOutput.GeneralId).ToArray()[0].Name);
				Console.WriteLine("[1] Input: {0}", inputOutput.Input == null ? "null" : ManageMaterial.NormalizateItems(inputOutput.Input.Split(';')));
				Console.WriteLine("[2] Output: {0}", inputOutput.Output == null ? "null" : ManageMaterial.NormalizateItems(inputOutput.Output.Split(';')));
				Console.WriteLine("[3] Production Time: {0}", inputOutput.ProductionTime);
				Console.WriteLine("Weight: {0}", inputOutput.Weight);
				Console.WriteLine("[9] Save");
				Console.WriteLine("[0] Exit");

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;

				Console.Clear();

				switch (select)
				{
					case '0': break;
					case '1': inputOutput.Input = ManageMaterial.SetItems(); break;
					case '2': inputOutput.Output = ManageMaterial.SetItems(false, true); break;
					case '3': inputOutput.ProductionTime = SetValue("Production Time"); break;
					case '9': inputOutput.Save(); break;
					default: Formations.NotFound("Action"); break;
				}
			}
			while (select != '0');
		}

		private static string SetValue(string header)
		{
			Console.Clear();
			Console.WriteLine("═════════╡ ENTER {0} ╞═════════", header.ToUpper());
			Console.Write("> ");
			return Console.ReadLine();
		}
	}
}
