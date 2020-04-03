using System;
using System.Collections.Generic;
using System.Text;

namespace MindustryLibrary
{
	public class Item
	{
		public override string ToString()
		{
			return $"{Material.GetMaterial(Id).Name} {Amount}";
		}

		public string Id { get; set; }
		public double Amount { get; set; }
	}
}