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

				if (select == '0')
				{
					return;
				}
				else if (select == '1')
				{
					string input = ManageMaterial.SetItems();
					if (input != "") inputOutput.Input = input;
				}
				else if (select == '2')
				{
					string output = ManageMaterial.SetItems(false, true);
					if (output != "") inputOutput.Output = output;
				}
				else if (select == '3')
				{
					inputOutput.ProductionTime = SetValue("Production Time");
				}
				else if (select == '9')
				{
					inputOutput.Save();
					return;
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}

		private static string SetValue(string header)
		{
			Console.WriteLine("═════════╡ ENTER {0} ╞═════════", header.ToUpper());
			Console.Write("> ");
			string value = Console.ReadLine();
			Console.Clear();
			return value;
		}
	}
}
