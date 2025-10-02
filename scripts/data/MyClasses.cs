using System;
using Godot;
using MyClasses;
using MyEnums;

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
	
	public static class Converters {
		public static Texture2D BiomeToTexture(Biomes biome)
		{
			return biome switch
			{
				Biomes.Woodland => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-woodlands.png"),
				Biomes.Swamp => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-swamp.png"),
				Biomes.Desert => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-desert.png"),
				Biomes.Mountain => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-woodlands.png"), // placeholder
				Biomes.Cave => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-cave.png"),
				Biomes.IceCave => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-icecave.png"),
				Biomes.Volcanic => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-volcanic.png"),
				Biomes.Ruins => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-woodlands.png"), // placeholder
				Biomes.Crypt => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-crypt.png"),
				Biomes.Abyss => GD.Load<Texture2D>("res://assets/sprites/menu/icons/biomes/spr-icon-biome-abyss.png"),
				_ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
			};
		}
	}
}
