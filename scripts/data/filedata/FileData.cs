using Godot;
using System;
using System.Text.Json;
using Items;
using MyEnums;

// Core data handling namespace
namespace FileData
{
	/// <summary>
	/// GameData class to store game-related data such as gold, wave, biome, and kill counts.
	/// </summary> 
	public class GameData
	{
		public int Gold { get; set; } = 0;
		public int Wave { get; set; } = 0;
		public Biomes Biome { get; set; } = Biomes.Woodland;
		public int Kills { get; set; } = 0;
		public int KillsHeavy { get; set; } = 0;
		public int ContractSeed { get; set; } = 0;
		public Item[] MarketItems { get; set; } = new Item[30];

		// Empty constructor for serialization
		public GameData() { }

		/// <summary> Constructor for GameData, optionally loading existing data. </summary>
		/// <param name="instantLoad">If true, loads data immediately on default.</param>
		public GameData(bool instantLoad = true)
		{
			if (instantLoad)
				Load();
		}

		/// <summary> Add a specified amount of gold to the current total. </summary>
		/// <param name="amount">The amount of gold to add.</param>
		public void AddGold(int amount)
		{
			GD.Print($"Adding {amount} gold.");
			Gold += amount;
		}
		
		public void RemoveGold(int amount)
		{
			GD.Print($"Removing {amount} gold.");
		}

		/// <summary> Increment the wave count by 1. </summary>
		public void AddWave()
		{
			GD.Print("Adding wave.");
			Wave++;
		}

		/// <summary> Add a kill to the kill count, specifying if it was a heavy enemy. </summary>
		/// <param name="isHeavy">True if the kill was a heavy enemy.</param>
		public void AddKill(bool isHeavy)
		{
			GD.Print("Adding kill; isHeavy: " + isHeavy);
			Kills++;
			KillsHeavy += (isHeavy) ? 1 : 0;
		}
		
		public bool AddMarketItem(Item item)
		{
			for (int i = 0; i < MarketItems.Length; i++)
			{
				if (MarketItems[i] == null)
				{
					item.Index = i;
					MarketItems[i] = item;
					return true;
				}
			}
			return false;
		}
		
		public void RemoveMarketItem(int index)
		{
			if (index >= 0 && index < MarketItems.Length)
			{
				MarketItems[index] = null;
				return;
			}
		}

		/// <summary>
		/// Save the current state of GameData to a file.
		/// </summary>
		public void Save()
		{
			GD.Print("Saving GameData");
			DataManager.SaveData("GameData", this);
		}

		/// <summary>
		/// Load the state of GameData from a file.
		/// </summary>
		public void Load()
		{
			var data = DataManager.LoadData<GameData>("GameData");
			if (data == null) { GD.Print("No GameData saved found! creating empty"); return; }
			Gold = data.Gold;
			Wave = data.Wave;
			Biome = data.Biome;
			Kills = data.Kills;
			KillsHeavy = data.KillsHeavy;
			MarketItems = data.MarketItems;
		}

		public void RandomizeContractSeed()
		{
			ContractSeed = new Random().Next();
			GD.Print($"New contract seed: {ContractSeed}");
		}

		public override string ToString()
		{
			string Return = string.Format(
				"== GAME DATA == \nGold: {0}\nWave: {1}\nBiome: {2}\nKills: {3} (Heavy: {4})\nContractSeed: {5}\nMarket Items({6}):\n",
						Gold, Wave, Biome, Kills, KillsHeavy, ContractSeed, MarketItems.Length);
			foreach (Item item in MarketItems)
			{
				if (item != null)
					Return += $"[{Array.IndexOf(MarketItems, item)}] {item.ToString()}\n";
			}
			return Return;
		}
	}

	/// <summary>
	/// PermData class to store permanent data such as gems.
	/// </summary> 
	public class PermData
	{
		public int[] Gems { get; set; } = { 0, 0, 0 };

		// Empty constructor for serialization
		public PermData() { }

		public PermData(bool instantLoad = true)
		{
			GD.Print("PermData Init");
			if (instantLoad)
				Load();
		}
		public void AddGem(int index)
		{
			if (index < 0 || index >= Gems.Length) throw new ArgumentOutOfRangeException(nameof(index), "Invalid gem index.");
			Gems[index]++;
		}

		/// <summary>
		/// Save the current state of PermData to a file.
		/// </summary>
		public void Save()
		{
			GD.Print("Saving PermData");
			DataManager.SaveData("PermData", this);
		}

		/// <summary>
		/// Load the state of PermData from a file.
		/// </summary>
		public void Load()
		{
			var data = DataManager.LoadData<PermData>("PermData");
			if (data == null) { GD.Print("No PermData saved found!, creating empty"); return; }
			Gems = data.Gems;
		}

		public override string ToString()
		{
			return $"== PERM DATA ==\nGems: [{string.Join(", ", Gems)}]";
		}
	}

	/// <summary> InventoryData class to manage the player's inventory of items. </summary>
	public class InventoryData
	{
		public Item[] Items { get; set; } = new Item[40];

		// Empty constructor for serialization
		public InventoryData() { }
		
		public InventoryData(bool instantLoad = true)
		{
			GD.Print("InventoryData Init");
			if (instantLoad)
				Load();
		}

		/// <summary> Add an item to the inventory. </summary>
		/// <returns>True if the item was added successfully, false if the inventory is full.</returns>
		public bool AddItem(Item newItem)
		{
			for (int i = 0; i < Items.Length; i++)
			{
				if (Items[i] == null)
				{
					newItem.Index = i;
					Items[i] = newItem;
					return true;
				}
			}
			return false;
		}

		/// <summary> Remove an item from the inventory by its index. </summary>
		/// <param name="index">The index of the item to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is invalid.</exception>
		public void RemoveItem(int index)
		{
			if (index >= 0 && index < Items.Length)
			{
				Items[index] = null;
				return;
			}
			throw new ArgumentOutOfRangeException(nameof(index), "Invalid inventory index.");
		}

		/// <summary> Save the current state of InventoryData to a file. </summary>
		public void Save()
		{
			GD.Print("Saving InventoryData");
			DataManager.SaveData("InventoryData", this);
		}

		/// <summary> Load the state of InventoryData from a file. </summary>
		public void Load()
		{
			var data = DataManager.LoadData<InventoryData>("InventoryData");
			if (data == null) { GD.Print("No InventoryData saved found!, creating empty"); return; }
			Items = data.Items;
		}

		public override string ToString()
		{
			var itemNames = "";
			for (int i = 0; i < Items.Length; i++)
			{
				if (Items[i] != null)
					itemNames += $"[{i}] {Items[i].Name}\n";
			}
			return $"== INVENTORY DATA ==\n" +
			   $"Items ({Items.Length}):\n" +
			   itemNames;
		}
	}

	/// <summary> Static class to manage loading and saving data to files. </summary>
	public static class DataManager
	{
		private static string PathOf(string name) => $"user://{name}.json";

		// Add shared serializer options
		private static readonly System.Text.Json.JsonSerializerOptions Options = new()
		{
			WriteIndented = true,
			DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
		};

		/// <summary> Load the state of a data object from a file. </summary>
		public static T LoadData<T>(string fileName)
		{
			GD.Print($"Loading {fileName} data.");
			var path = PathOf(fileName);
			if (!FileAccess.FileExists(path)) return default;

			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			string json = f.GetAsText();
			return System.Text.Json.JsonSerializer.Deserialize<T>(json, Options);
		}

		/// <summary> Save the state of a data object to a file. </summary>
		public static void SaveData<T>(string fileName, T data)
		{
			GD.Print($"Saving {fileName} data.");
			var path = PathOf(fileName);
			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Write);
			f.StoreString(System.Text.Json.JsonSerializer.Serialize(data, Options));
		}

		/// <summary> Delete the saved GameData file. </summary>
		public static void DeleteGameData()
		{
			GD.Print("Deleting GameData file.");
			var path = PathOf("GameData");
			if (FileAccess.FileExists(path))
				DirAccess.RemoveAbsolute(path);
			else
				GD.Print("No GameData file to delete.");
		}

		/// <summary> Delete the saved PermData file. </summary>
		public static void DeletePermData()
		{
			GD.Print("Deleting PermData file.");
			var path = PathOf("PermData");
			if (FileAccess.FileExists(path))
				DirAccess.RemoveAbsolute(path);
			else
				GD.Print("No PermData file to delete.");
		}

		/// <summary> Delete the saved InventoryData file. </summary>
		public static void DeleteInventoryData()
		{
			GD.Print("Deleting InventoryData file.");
			var path = PathOf("InventoryData");
			if (FileAccess.FileExists(path))
				DirAccess.RemoveAbsolute(path);
			else
				GD.Print("No InventoryData file to delete.");
		}
	}
}
