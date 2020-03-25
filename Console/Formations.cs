using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindustryConsole
{
	class Formations
	{
		public static void NotFound(string text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("{0} NOT FOUND!!!", text.ToUpper());
			Console.ForegroundColor = ConsoleColor.Gray;
		}
		public static void NotCorrect(string text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("{0} NOT CORRECT!!!", text.ToUpper());
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static void Header(string text)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(text);
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		public static string SetValue(string header, string type)
		{
			Console.Clear();
			Console.WriteLine("═════════╡ ENTER {0} ╞═════════", header.ToUpper());
			Console.Write("> ");
			string select = Console.ReadLine();

			if (select == "null")
				return null;
			else
				return select;
		}

		public static int GetInt(string text)
		{
			try
			{
				return Convert.ToInt32(text);
			}
			catch
			{
				return -1;
			}
		}
		public static double GetDouble(string text)
		{
			try
			{
				return Convert.ToDouble(text);
			}
			catch
			{
				return -1;
			}
		}
	}
}
