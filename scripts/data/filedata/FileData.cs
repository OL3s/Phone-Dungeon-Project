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
		public Item Equiped { get; set; } = null;
		public Item EquipedUpper { get; set; } = null;
		public bool UpdateMarket { get; set; } = true;

		// Empty constructor for serialization
		public GameData() { GD.Print("[GameData] Empty GameData for Json loader"); }

		/// <summary> Constructor for GameData, optionally loading existing data. </summary>
		/// <param name="instantLoad">If true, loads data immediately on default.</param>
		public GameData(bool instantLoad)
		{
			GD.Print("[GameData] Init");
			if (instantLoad)
				Load();
		}

		/// <summary> Add a specified amount of gold to the current total. </summary>
		/// <param name="amount">The amount of gold to add.</param>
		public void AddGold(int amount)
		{
			GD.Print($"[GameData] Adding {amount} gold. (Current Gold: {Gold})");
			Gold += amount;
		}
		
		public void RemoveGold(int amount)
		{
			GD.Print($"[GameData] Removing {amount} gold. (Current Gold: {Gold})");
			Gold -= amount;
		}

		/// <summary> Increment the wave count by 1. </summary>
		public void AddWave()
		{
			GD.Print("[GameData] Adding wave.");
			Wave++;
		}

		/// <summary> Add a kill to the kill count, specifying if it was a heavy enemy. </summary>
		/// <param name="isHeavy">True if the kill was a heavy enemy.</param>
		public void AddKill(bool isHeavy)
		{
			GD.Print($"[GameData] Adding kill; isHeavy: {isHeavy}");
			Kills++;
			KillsHeavy += (isHeavy) ? 1 : 0;
		}
		
		public bool AddMarketItem(Item item)
		{
			GD.Print($"[GameData] Adding MarketItem: {item.Name}");

			// Is null debugger
			if (item == null)
				throw new ArgumentNullException(nameof(item), "Item cannot be null.");
		
			for (int i = 0; i < MarketItems.Length; i++)
			{
				if (MarketItems[i] == null)
				{
					item.Index = i;
					MarketItems[i] = item;
					GD.Print($"Adding MarketItem: {item.Name}, {item.Index ?? -1}");
					return true;
				}
			}
			return false;
		}
		
		public void RemoveMarketItem(int index)
		{
			GD.Print($"[GameData] Removing MarketItem at index {index}");

			// Out of bounds debugger
			if (!(index >= 0 && index < MarketItems.Length))
				throw new ArgumentOutOfRangeException(nameof(index), "Invalid MarketItem index.");

			// Is null debugger
			if (MarketItems[index] == null)
				GD.PrintErr("Tried to remove a MarketItem which is already null");
	
			// remove index
			MarketItems[index] = null;
		}

		public void SetEquipped(Item item, bool isUpper) 
		{
			GD.Print($"[GameData] Setting equipped item: {item?.Name}, isUpper: {isUpper}");
			if (item == null)
				throw new ArgumentNullException(nameof(item), "Item cannot be null.");

			// Prevent duplicate: if equipping to upper, remove from lower if same item, and vice versa
			if (isUpper)
			{
				if (Equiped != null && Equiped.Name == item.Name)
					Equiped = null;
				EquipedUpper = item;
			}
			else
			{
				if (EquipedUpper != null && EquipedUpper.Name == item.Name)
					EquipedUpper = null;
				Equiped = item;
			}
		}

		public void RemoveEquipped(bool isUpper) 
		{
			GD.Print($"[GameData] Removing equipped item, isUpper: {isUpper}");
			if (isUpper)
				EquipedUpper = null;
			else
				Equiped = null;
		}

		public int GetEquippedIndex(Item item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item), "Item cannot be null.");
			if (Equiped != null && Equiped.Name == item.Name)
				return 1;
			if (EquipedUpper != null && EquipedUpper.Name == item.Name)
				return 2;
			return 0;
		}

		/// <summary>
		/// Save the current state of GameData to a file.
		/// </summary>
		public void Save()
		{
			GD.Print("[GameData] Saving GameData");
			DataManager.SaveData("GameData", this);
		}

		/// <summary>
		/// Load the state of GameData from a file.
		/// </summary>
		public void Load()
		{
			GD.Print("[GameData] Loading GameData");
			var data = DataManager.LoadData<GameData>("GameData");
			if (data == null) { GD.Print("No GameData saved found! creating empty"); return; }
			Gold = data.Gold;
			Wave = data.Wave;
			Biome = data.Biome;
			Kills = data.Kills;
			KillsHeavy = data.KillsHeavy;
			MarketItems = data.MarketItems;
			Equiped = data.Equiped;
			EquipedUpper = data.EquipedUpper;
		}

		public void RandomizeContractSeed()
		{
			ContractSeed = new Random().Next();
			GD.Print($"[GameData] New contract seed: {ContractSeed}");
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
		public PermData() { GD.Print("[PermData] Empty PermData for Json loader"); }

		public PermData(bool instantLoad)
		{
			GD.Print("[PermData] Init");
			if (instantLoad)
				Load();
		}
		public void AddGem(int index)
		{
			GD.Print($"[PermData] Adding gem at index {index}");
			// Out of bounds debugger
			if (index < 0 || index >= Gems.Length) throw new ArgumentOutOfRangeException(nameof(index), "Invalid gem index.");
			Gems[index]++;
		}

		/// <summary>
		/// Save the current state of PermData to a file.
		/// </summary>
		public void Save()
		{
			GD.Print("[PermData] Saving PermData");
			DataManager.SaveData("PermData", this);
		}

		/// <summary>
		/// Load the state of PermData from a file.
		/// </summary>
		public void Load()
		{
			GD.Print("[PermData] Loading PermData");
			var data = DataManager.LoadData<PermData>("PermData");
			if (data == null) { GD.Print("[PermData] No PermData saved found!, creating empty"); return; }
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
		public InventoryData() { GD.Print("[InventoryData] Empty InventoryData for Json loader"); }

		public InventoryData(bool instantLoad)
		{
			GD.Print("[InventoryData] Init");
			if (instantLoad)
				Load();
		}

		/// <summary> Add an item to the inventory. </summary>
		/// <returns>Index of added item OR -1 if failed</returns>
		public int AddItem(Item newItem)
		{
			GD.Print($"[InventoryData] Adding item {newItem.Name} to inventory.");
			for (int i = 0; i < Items.Length; i++)
			{
				if (Items[i] == null)
				{
					newItem.Index = i;
					Items[i] = newItem;
					return i;
				}
			}
			return -1;
		}

		/// <summary> Remove an item from the inventory by its index. </summary>
		/// <param name="index">The index of the item to remove.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the index is invalid.</exception>
		public void RemoveItem(int index)
		{
			GD.Print("[InventoryData] Removing item from inventory.");

			// Out of bounds debugger
			if (!(index >= 0 && index < Items.Length))
				throw new ArgumentOutOfRangeException(nameof(index), "Invalid inventory index.");
				
			// remove index
			Items[index] = null;	
		}
		
		public void RemoveItem(Item item)
		{
			GD.Print("[InventoryData] Removing item from inventory.");

			// Is null debugger
			int? index = item.Index;
			if (index == null) 
				throw new InvalidOperationException("Item has no index.");
			
			// Out of bounds debugger
			int _index = item.Index.Value;
			if (!(_index >= 0 && _index < Items.Length))
				throw new ArgumentOutOfRangeException(nameof(index), "Invalid inventory index.");
				
			// remove index
			Items[_index] = null;
		}

		public static bool EqualsItemAndIndex(Item item, int index)
		{
			if (item.Index == null) return false;
			return item.Index == index;
		}

		/// <summary> Save the current state of InventoryData to a file. </summary>
		public void Save()
		{
			GD.Print("[InventoryData] Saving InventoryData");
			DataManager.SaveData("InventoryData", this);
		}

		/// <summary> Load the state of InventoryData from a file. </summary>
		public void Load()
		{
			GD.Print("[InventoryData] Loading InventoryData");
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
			GD.Print($"[DataManager] Loading {fileName} data.");
			var path = PathOf(fileName);
			if (!FileAccess.FileExists(path)) 
			{
				GD.PrintErr($"[DataManager] File {fileName} does not exist, returning default.");
				return default;
			}

			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			if (f == null)
			{
				GD.PrintErr($"[DataManager] Failed to open {fileName} for reading!");
				return default;
			}
			
			string json = f.GetAsText();
			var result = System.Text.Json.JsonSerializer.Deserialize<T>(json, Options);
			GD.Print($"[DataManager] Successfully loaded {fileName} data.");
			return result;
		}

		/// <summary> Save the state of a data object to a file. </summary>
		public static void SaveData<T>(string fileName, T data)
		{
			GD.Print($"[DataManager] Saving {fileName} data.");
			var path = PathOf(fileName);
			
			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Write);
			if (f == null)
			{
				GD.PrintErr($"[DataManager] Failed to open {fileName} for writing!");
				return;
			}
			
			f.StoreString(System.Text.Json.JsonSerializer.Serialize(data, Options));
			GD.Print($"[DataManager] Successfully saved {fileName} data.");
		}

		public static void DeleteData(string fileName)
		{
			GD.Print($"[DataManager] Deleting {fileName} data.");
			var path = PathOf(fileName);
			if (FileAccess.FileExists(path))
				DirAccess.RemoveAbsolute(path);
			else
				GD.Print($"[DataManager] No {fileName} file to delete.");
		}

		/// <summary> Delete the saved GameData file. </summary>
		public static void DeleteGameData()
		{
			DeleteData("GameData");
		}

		/// <summary> Delete the saved PermData file. </summary>
		public static void DeletePermData()
		{
			DeleteData("PermData");
		}

		/// <summary> Delete the saved InventoryData file. </summary>
		public static void DeleteInventoryData()
		{
			DeleteData("InventoryData");
		}
	}
}
