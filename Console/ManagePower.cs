using MindustryLibrary;
using System;
using System.Linq;

namespace MindustryConsole
{
	class ManagePower
	{
		public static void Update(Power power)
		{
			char select;

			do
			{
				Console.WriteLine("═════════╡ UPDATE POWER ╞═════════");
				Console.WriteLine("ID: {0}", power.Id);
				Console.WriteLine("General: {0}", power.GetGeneral.Name);
				Console.WriteLine("[1] Input Output: {0}", power.InputOutputId);
				Console.WriteLine("[2] Power Use: {0}", power.PowerUse);
				Console.WriteLine("[3] Power Capacity: {0}", power.PowerCapacity);
				Console.WriteLine("[4] Power Generation: {0}", power.PowerGeneration);
				Console.WriteLine("[8] Delete");
				Console.WriteLine("[9] Save");
				Console.WriteLine("[0] Exit");

				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0')
				{
					bool allCorrect = true;

					if (power.PowerUse != null || power.PowerCapacity != null || power.PowerGeneration != null)
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
					string id = SetInputOutput(power.GetGeneral.GetInputOutputs);

					if (id != "")
						power.InputOutputId = id;
				}
				else if (select == '2')
					power.PowerUse = Formations.GetValue("Power Use", "double");
				else if (select == '3')
					power.PowerCapacity = Formations.GetValue("Power Capacity", "double");
				else if (select == '4')
					power.PowerGeneration = Formations.GetValue("Power Generation", "double");
				else if (select == '8' && Delete(power))
					return;
				else if (select == '9')
				{
					bool allCorrect = true;

					if (power.PowerUse == null && power.PowerCapacity == null && power.PowerGeneration == null)
					{
						allCorrect = false;
						Formations.NotCorrect("Power Use or Capacity or Generation");
					}

					if (allCorrect)
					{
						power.Save();
						return;
					}
				}
				else
					Formations.NotFound("Action");
			}
			while (true);
		}
		private static bool Delete(Power power)
		{
			char select;

			Console.WriteLine("═════════╡ TO DELETE {0}? (Y - YES)╞═════════", power.ToString().ToUpper());
			Console.Write("> ");
			select = Console.ReadKey().KeyChar;
			Console.Clear();

			if (select.ToString().ToLower() == "y")
			{
				power.Delete();
				return true;
			}

			return false;
		}


		private static string SetInputOutput(InputOutput[] inputOutputs)
		{
			int offset = 2;
			int select;

			do
			{
				Console.WriteLine("╔═══╤╤═╡ SET ITEMS ╞══════╗");
				Console.WriteLine("║ 0 ├┤ Cancel             ║");
				Console.WriteLine("║ 1 ├┤ Set Null           ║");
				Console.WriteLine("╚═══╧╧════════════════════╝");

				for (int i = 0; i < inputOutputs.Length; i++)
					Console.WriteLine("║{0}├┤ {1}", i + offset, inputOutputs[i]);

				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0)
					return "";
				else if (select == 1)
					return null;
				else if (select >= offset && select < inputOutputs.Length + offset)
					return inputOutputs[select - offset].Id;
				else
					Formations.NotCorrect("Action");
			}
			while (true);
		}
	}
}