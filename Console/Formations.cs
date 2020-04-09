using System;
using System.Linq;

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

		public static string GetValue(string header, string type)
		{
			Console.WriteLine("═════════╡ ENTER {0} ╞═════════", header.ToUpper());
			Console.Write("> ");
			string text = Console.ReadLine();
			Console.Clear();

			if (type == "double")
			{
				double value = GetDouble(text);
				
				if (value == -1) text = "-1";
				else if (value == 0) text = null;
				else return value.ToString();
			}
			else if (type == "int")
			{
				int value = GetInt(text);

				if (value == -1) text = "-1";
				else if (value == 0) text = null;
				else return value.ToString();
			}
			else if (type == "size")
			{
				string[] size = text.Split('x');

				if (size.Length == 2)
				{
					int x = GetInt(size.First());
					int y = GetInt(size.Last());

					if (x == -1 || y == -1) text = null;

					return text;
				}
				else if (text == "0") text = null;
				else text = "-1";
			}
			else if (type == "string")
			{
				if (text == "null") return null;

				return text;
			}
			else
			{
				NotFound($"type \"{type}\"");
				return null;
			}

			if (text == "-1")
				NotCorrect(header);
			return null;
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