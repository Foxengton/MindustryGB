using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindustryLibrary;

namespace MindustryConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			char select;
			do
			{
				Console.WriteLine("╔════════╡ MENU ╞════════╗");
				Console.WriteLine("╟─┐ ┌────────────────────╢");
				Console.WriteLine("║0├─┤Exit                ║");
				Console.WriteLine("║1├─┤Calculator          ║");
				Console.WriteLine("║2├─┤Manager Building    ║");
				Console.WriteLine("║3├─┤Manager Material    ║");
				Console.WriteLine("╟─┘ └────────────────────╢");
				Console.WriteLine("╚════════════════════════╝");
				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				if (select == '0') return;
				else if (select == '1') ManageCalculator.Menu();
				else if (select == '2') ManageBuilding.Menu();
				else if (select == '3') ManageMaterial.Menu();
				else Formations.NotFound("Action");
			}
			while (true);
		}
	}
}
