using System;

namespace MindustryLibrary
{
	public class Item
	{
		public override string ToString()
		{
			return $"{Material.GetMaterial(Id).Name} {Math.Round(Amount * 100) / 100}";
		}

		public string Id { get; set; }
		public double Amount { get; set; }
	}
}