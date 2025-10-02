using Godot;
using System;
using System.Text.Json;
using MyClasses;
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

		/// <summary>
		/// Save the current state of GameData to a file.
		/// </summary>
		public void Save()
		{
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
		}

		public void RandomizeContractSeed()
		{
			ContractSeed = new Random().Next();
			GD.Print($"New contract seed: {ContractSeed}");
		}
	}

	/// <summary>
	/// PermData class to store permanent data such as gems.
	/// </summary> 
	public class PermData
	{
		public int[] Gems { get; set; } = { 0, 0, 0 };

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
	}

	/// <summary> InventoryData class to manage the player's inventory of items. </summary>
	public class InventoryData
	{
		public Item[] Items { get; set; } = new Item[40];
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
			DataManager.SaveData("InventoryData", this);
		}

		/// <summary> Load the state of InventoryData from a file. </summary>
		public void Load()
		{
			var data = DataManager.LoadData<InventoryData>("InventoryData");
			if (data == null) { GD.Print("No InventoryData saved found!, creating empty"); return; }
			Items = data.Items;
		}
	}

	/// <summary> Static class to manage loading and saving data to files. </summary>
	public static class DataManager
	{
		private static string PathOf(string name) => $"user://{name}.json";

		/// <summary> Load the state of a data object from a file. </summary>
		public static T LoadData<T>(string fileName)
		{
			GD.Print($"Loading {fileName} data.");
			var path = PathOf(fileName);
			if (!FileAccess.FileExists(path)) return default;

			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			string json = f.GetAsText();
			return JsonSerializer.Deserialize<T>(json);
		}

		/// <summary> Save the state of a data object to a file. </summary>
		public static void SaveData<T>(string fileName, T data)
		{
			GD.Print($"Saving {fileName} data.");
			var path = PathOf(fileName);
			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Write);
			f.StoreString(JsonSerializer.Serialize(data));
		}

		/// <summary> Delete all saved data files. </summary>
		public static void DeleteAllData()
		{
			GD.Print("Deleting all data files.");
			DeleteGameData();
			DeletePermData();
			DeleteInventoryData();
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
