using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindustryLibrary;

namespace MindustryConsole
{
	class ManageType
	{
		public static string SetType(string kind)
		{
			int offset = 1;
			int select;

			TypeItem[] typeItems = TypeItem.GetKind(kind);
			if (typeItems.Length == 0)
				return null;

			do
			{
				Console.WriteLine("╔══╤╤═╡ SET MOD ╞═════╗");
				Console.WriteLine("║0 ├┤ Cancel          ║");
				for (int i = 0; i < typeItems.Length; i++)
					Console.WriteLine("║{0,2}├┤ {1,15} ║", (i + offset).ToString().PadRight(2, ' '), typeItems[i].ToString().PadRight(15, ' '));
				Console.WriteLine("╚══╧╧═════════════════╝");
				Console.Write("> ");
				select = Formations.GetInt(Console.ReadLine());
				Console.Clear();

				if (select == 0)
					return null;
				else if (select >= offset && select < typeItems.Length + offset)
					return typeItems[select - offset].Id;
				else
					Formations.NotCorrect("Action");
			}
			while (true);
		}
	}
}
