using MindustryLibrary;
using System;
using System.Linq;

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
				Console.WriteLine("Owner: {0}", General.Generals.Where(gen => gen.Id == inputOutput.GeneralId).ToArray()[0].Name);
				Console.WriteLine("[1] Input: {0}", inputOutput.Input == null ? "null" : ManageMaterial.NormalizateItems(inputOutput.Input));
				Console.WriteLine("[2] Output: {0}", inputOutput.Output == null ? "null" : ManageMaterial.NormalizateItems(inputOutput.Output));
				Console.WriteLine("[3] Production Time: {0}", inputOutput.ProductionTime);
				Console.WriteLine("Weight: {0}", inputOutput.Weight);
				Console.WriteLine("[9] Save");
				Console.WriteLine("[0] Exit");

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0') return;
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
				else if (select == '3') inputOutput.ProductionTime = Formations.SetValue("Production Time", "double");
				else if (select == '9')
				{
					inputOutput.Save();
					return;
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}
	}
}