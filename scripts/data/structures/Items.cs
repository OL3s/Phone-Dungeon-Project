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

		public Item() { }

		public static Item ItemPreset(ItemPresets preset)
		{
			// This method would return an item based on the preset enum
			// Implementation would depend on how presets are defined
			return preset switch
			{
				ItemPresets.Sword => new Weapon("res://path/to/sword.png", "Sword", 100, null),
				ItemPresets.Axe => new Weapon("res://path/to/axe.png", "Axe", 120, null),
				ItemPresets.Bow => new Weapon("res://path/to/bow.png", "Bow", 150, null),
				ItemPresets.Dagger => new Weapon("res://path/to/dagger.png", "Dagger", 80, null),
				ItemPresets.Staff => new Weapon("res://path/to/staff.png", "Staff", 200, null),
				ItemPresets.HealthPotion => new Item("res://path/to/health_potion.png", "Health Potion", 50),
				ItemPresets.ManaPotion => new Item("res://path/to/mana_potion.png", "Mana Potion", 60),
				ItemPresets.StaminaPotion => new Item("res://path/to/stamina_potion.png", "Stamina Potion", 55),
				ItemPresets.IronPlate => new Item("res://path/to/iron_plate.png", "Iron Plate Armor", 300),
				ItemPresets.LeatherArmor => new Item("res://path/to/leather_armor.png", "Leather Armor", 200),
				ItemPresets.MagicRobe => new Item("res://path/to/magic_robe.png", "Magic Robe", 250),
				ItemPresets.RingOfStrength => new Item("res://path/to/ring_of_strength.png", "Ring of Strength", 400),
				ItemPresets.RingOfAgility => new Item("res://path/to/ring_of_agility.png", "Ring of Agility", 400),
				ItemPresets.RingOfWisdom => new Item("res://path/to/ring_of_wisdom.png", "Ring of Wisdom", 400),
				_ => throw new ArgumentException("Invalid item preset"),
			};
		}

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
			return $"[Item] ToString: Name: {Name}, Cost: {Cost}, Condition: {Condition}, Index: {Index}";
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

	public enum ItemPresets
	{
		// Weapons
		Sword,
		Axe,
		Bow,
		Dagger,
		Staff,

		// Consumables
		HealthPotion,
		ManaPotion,
		StaminaPotion,

		// Armor
		IronPlate,
		LeatherArmor,
		MagicRobe,

		// Rings
		RingOfStrength,
		RingOfAgility,
		RingOfWisdom,

	}

}
