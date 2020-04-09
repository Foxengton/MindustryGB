using MindustryLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindustryConsole
{
	class ManageMod
	{
		public static string SetMod()
		{
			int offset = 1;
			int select;

			do
			{
				Console.WriteLine("╔═╤╤═╡ SET MOD ╞════╗");
				Console.WriteLine("║0├┤ Cancel         ║");
				for (int i = 0; i < Mod.Count; i++)
					Console.WriteLine("║{0}├┤ {1,14} ║", i + offset, Mod.Mods[i].ToString().PadRight(14, ' '));
				Console.WriteLine("╚═╧╧════════════════╝");
				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0)
					return null;
				else if (select >= offset && select < Mod.Count + offset)
					return Mod.Mods[select - offset].Id;
				else
					Formations.NotCorrect("Action");
			}
			while (true);
		}
	}
}
