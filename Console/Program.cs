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
				Console.WriteLine("║1├─┤Manager Building    ║");
				Console.WriteLine("║2├─┤Manager Material    ║");
				Console.WriteLine("╟─┘ └────────────────────╢");
				Console.WriteLine("╚════════════════════════╝");
				Console.Write("> ");
				select = Console.ReadKey().KeyChar;
				Console.Clear();

				switch (select)
				{
					case '0': break;
					case '1': ManageBuilding.Menu(); break;
					case '2': ManageMaterial.Menu(); break;
					default: Formations.NotFound("Action"); break;
				}
			}
			while (select != '0');
		}
	}
}
