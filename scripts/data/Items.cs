using System;
using Godot;
using MyEnums;
using Combat;
using System.Text.Json.Serialization; 

namespace Items
{
	// Enable polymorphism for different item types
	[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
	[JsonDerivedType(typeof(Item), "item")]
	[JsonDerivedType(typeof(Weapon), "weapon")]


	public class Item
	{
		public string TexturePath { get; set; }
		public string Name { get; }
		public int Cost { get; }
		public int Condition { get; set; }
		public int? Index { get; set; }

		public Item(string texturePath, string name, int cost, int condition = 100, int? index = null)
		{
			Index = index;
			TexturePath = texturePath;
			Name = name;
			Cost = cost;
			Condition = condition;
		}
		
		public override string ToString() 
		{
			return $"itemObject: Name: {Name}, Cost: {Cost}, Condition: {Condition}, Index: {Index}";
		}
	}

	public class Weapon : Item
	{
		public Attack AttackData { get; set; }

		public Weapon(string texturePath, string name, int cost, Attack attackData, int condition = 100) 
			: base(texturePath, name, cost, condition)
		{
			AttackData = attackData;
		}
	}

}
