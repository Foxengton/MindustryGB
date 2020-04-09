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
				Console.WriteLine("Owner: {0}", General.Generals.Where(gen => gen.Id == inputOutput.GeneralId).First().Name);
				Console.WriteLine("[1] Input: {0}", inputOutput.Inputs == null ? "null" : string.Join(", ", inputOutput.Inputs.Select(sel => sel.ToString())));
				Console.WriteLine("[2] Output: {0}", inputOutput.Outputs == null ? "null" : string.Join(", ", inputOutput.Outputs.Select(sel => sel.ToString())));
				Console.WriteLine("[3] Production Time: {0}", inputOutput.ProductionTime);
				Console.WriteLine("Weight: {0}", inputOutput.Weight);
				Console.WriteLine("[8] Delete");
				Console.WriteLine("[9] Save");
				Console.WriteLine("[0] Exit");

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0')
				{
					bool allCorrect = true;

					if (inputOutput.ProductionTime != null || inputOutput.Inputs != null || inputOutput.Outputs != null)
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
					Item[] input = ManageMaterial.SetItems();
					if (input != null) inputOutput.Input = string.Join(";", input.Select(inp => inp.Id + " " + inp.Amount));
				}
				else if (select == '2')
				{
					Item[] output = ManageMaterial.SetItems(true);
					if (output != null) inputOutput.Output = string.Join(";", output.Select(inp => inp.Id + " " + inp.Amount));
				}
				else if (select == '3') inputOutput.ProductionTime = Formations.GetValue("Production Time", "double");
				else if (select == '8')
				{
					Console.WriteLine("═════════╡ TO DELETE {0}? (Y - YES)╞═════════", inputOutput.ToString().ToUpper());
					Console.Write("> ");
					select = Console.ReadKey().KeyChar;
					Console.Clear();

					if (select.ToString().ToLower() == "y")
					{
						inputOutput.Delete();
						return;
					}
				}
				else if (select == '9')
				{
					bool allCorrect = true;

					if (inputOutput.ProductionTime == null)
					{
						allCorrect = false;
						Formations.NotCorrect("Production Time");
					}
					if (inputOutput.Inputs == null && inputOutput.Outputs == null)
					{
						allCorrect = false;
						Formations.NotCorrect("Input or Output");
					}

					if (allCorrect)
					{
						inputOutput.Save();
						return;
					}
				}
				else Formations.NotFound("Action");
			}
			while (true);
		}
	}
}