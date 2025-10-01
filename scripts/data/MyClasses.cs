using System;
using Godot;

namespace MyClasses {

	public class Item
	{
		public string Name { get; }
		public int Cost { get; }
		public int Condition { get; set; }
		
		public Item(string name, int cost, int condition = 100)
		{
			Name = name;
			Cost = cost;
			Condition = condition;
		}
	}
}
