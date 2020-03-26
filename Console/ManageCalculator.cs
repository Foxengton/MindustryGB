using MindustryLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindustryConsole
{
	class ManageCalculator
	{
		public static void Menu()
		{
			double amount = 0;
			int index = ManageMaterial.GetIndex();
			Console.Clear();

			if (index == -1) return;

			do
			{
				Console.WriteLine("═════════╡ ENTER AMOUNT OF {0} PER SECOND ╞═════════", Material.materials[index].Name.ToUpper());
				Console.Write("> ");
				amount = Formations.GetDouble(Console.ReadLine());
				Console.Clear();

				if (amount == -1) Formations.NotCorrect("Amount");
				else break;
			}
			while (true);

			Calculator(index, amount);
		}

		private static void Calculator(int index, double amount)
		{
			for (int i = 0; i < InputOutput.inputsOutputs.Length; i++)
			{
				string[] items = InputOutput.inputsOutputs[i].Output.Split(';');
				double item = Formations.GetDouble(items[index]);

				if (item != -1)
				{
					double productionTime = Convert.ToDouble(InputOutput.inputsOutputs[i].ProductionTime);
					double amountOfBuilding = Math.Ceiling(amount / (item / productionTime));
					Console.WriteLine("You need: {0} {1}", amountOfBuilding, General.generals.Where(gen => gen.Id == InputOutput.inputsOutputs[i].GeneralId).ToArray()[0].Name);
				}
			}
		}
	}
}
