using System;
using Godot;
using MyEnums;
using Combat;
using Items;

namespace MyClasses {

	public class Contract
	{
		public Biomes Biome { get; set; }
		public int Difficulty { get; set; }
		public MissionType Mission { get; set; }
		public int Reward { get; set; }

		/// <summary>
		/// Create a contract with specified parameters.
		/// </summary>
		/// <param name="biome"> The biome for the contract. </param>
		/// <param name="difficulty"> The difficulty level of the contract. </param>
		/// <param name="mission"> The type of mission for the contract. </param>
		/// <param name="reward"> The reward amount for completing the contract. </param>
		public Contract(Biomes biome, int difficulty, MissionType mission, int reward)
		{
			Biome = biome;
			Difficulty = difficulty;
			Mission = mission;
			Reward = reward;
		}

		/// <summary>
		/// Create a randomized contract based on biome and wave, using seed for randomness.
		/// </summary>
		/// <param name="biome"> The current biome. </param>
		/// <param name="wave"> The current wave. </param>
		/// <param name="seed"> The seed for random number generation. Use from GameData. </param>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public Contract(Biomes biome, int wave, int seed)
		{

			// Set random seed
			Random random = new Random(seed);

			Biome = biome switch
			{
				Biomes.Woodland => new[] { Biomes.Woodland, Biomes.Swamp, Biomes.Desert }[random.Next(0, 3)],
				Biomes.Swamp => new[] { Biomes.Swamp, Biomes.Desert, Biomes.Mountain }[random.Next(0, 3)],
				Biomes.Desert => new[] { Biomes.Desert, Biomes.Mountain, Biomes.Cave }[random.Next(0, 3)],
				Biomes.Mountain => new[] { Biomes.Mountain, Biomes.Cave, Biomes.IceCave }[random.Next(0, 3)],
				Biomes.Cave => new[] { Biomes.Cave, Biomes.IceCave, Biomes.Volcanic }[random.Next(0, 3)],
				Biomes.IceCave => new[] { Biomes.IceCave, Biomes.Volcanic, Biomes.Ruins }[random.Next(0, 3)],
				Biomes.Volcanic => new[] { Biomes.Volcanic, Biomes.Ruins, Biomes.Crypt }[random.Next(0, 3)],
				Biomes.Ruins => new[] { Biomes.Ruins, Biomes.Crypt, Biomes.Abyss }[random.Next(0, 3)],
				Biomes.Crypt => new[] { Biomes.Crypt, Biomes.Abyss, Biomes.Woodland }[random.Next(0, 3)],
				Biomes.Abyss => new[] { Biomes.Abyss, Biomes.Woodland, Biomes.Swamp }[random.Next(0, 3)],
				_ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
			};
			Difficulty = Mathf.Clamp(wave + random.Next(-2, 3), 1, 10);
			Mission = (random.NextDouble() < 0.5) ? (MissionType)random.Next(0, Enum.GetNames(typeof(MissionType)).Length) : MissionType.None;
			Reward = (Mission != MissionType.None) ? Difficulty * 100 : 0;
		}

	}

	public static class Converters
	{
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
