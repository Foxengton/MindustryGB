using System;

namespace MindustryConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			SqliteDataAccess.DataAccess();

			bool isAdministrator = false;
			char select;

			do
			{
				Console.WriteLine("╔════════╡ MENU ╞════════╗");
				Console.WriteLine("╟─┐ ┌────────────────────╢");
				Console.WriteLine("║0├─┤Exit                ║");
				Console.WriteLine("║1├─┤Calculator          ║");
				if (isAdministrator)
				{
					Console.WriteLine("║2├─┤Manager Building    ║");
					Console.WriteLine("║3├─┤Manager Material    ║");
					Console.WriteLine("║4├─┤Log Out             ║");
				}
				else
				{
					Console.WriteLine("║2├─┤Administrator       ║");
				}
				Console.WriteLine("╟─┘ └────────────────────╢");
				Console.WriteLine("╚════════════════════════╝");
				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0')
					return;
				else if (select == '1')
					ManageCalculator.Menu();
				else if (isAdministrator)
				{
					if (select == '2')
						ManageBuilding.Menu();
					else if (select == '3')
						ManageMaterial.Menu();
					else if (select == '4')
						isAdministrator = false;
					else
						Formations.NotFound("Action");

				}
				else
				{
					if (select == '2')
						isAdministrator = EnterPassword();
					else
						Formations.NotFound("Action");
				}
			}
			while (true);
		}

		static bool EnterPassword()
		{
			Console.WriteLine("════╡ ENTER PASSWORD ╞════");
			Console.Write("> ");
			string password = Console.ReadLine();
			Console.Clear();

			if (password == "fox")
				return true;
			else
				Formations.NotCorrect("Password");

			return false;
		}
	}
}