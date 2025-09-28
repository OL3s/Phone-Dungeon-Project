using Godot;
using System;
using System.Text.Json;
using MyClasses;
using MyEnums;

namespace FileData
{
	public class Storage
	{
		public GameData? GameData { get; private set; }
		public PermData? PermData { get; private set; }
		public InventoryData? InventoryData { get; private set; }

		public Storage(bool includeGame, bool includeGlobal, bool includeInventory)
		{
			if (includeGame)
				GameData = DataManager.LoadData<GameData>("GameData") ?? new GameData();

			if (includeGlobal)
				PermData = DataManager.LoadData<PermData>("PermData") ?? new PermData();

			if (includeInventory)
				InventoryData = DataManager.LoadData<InventoryData>("InventoryData") ?? new InventoryData();
		}

		public void SaveGame()       => GameData?.Save();
		public void SaveGlobal()     => PermData?.Save();
		public void SaveInventory()  => InventoryData?.Save();
		public void SaveAll() { SaveGame(); SaveGlobal(); SaveInventory(); }
	}

	public class GameData
	{
		public int Gold { get; set; }
		public int Wave { get; set; }
		public Biomes Biome { get; set; }
		public int Kills { get; set; }
		public int KillsHeavy { get; set; }

		public GameData(bool instantLoad = true)
		{
			GD.Print("New GameData created!");
			Gold = 0;
			Wave = 0;
			Biome = Biomes.Woodland;
			Kills = 0;
			KillsHeavy = 0;
			if (instantLoad)
				Load();
		}

		void AddGold(int amount)
		{
			GD.Print($"Adding {amount} gold.");
			Gold += amount;
		}

		void AddWave()
		{
			GD.Print("Adding wave.");
			Wave++;
		}

		void AddKill(bool isHeavy)
		{
			GD.Print("Adding kill; isHeavy: " + isHeavy);
			Kills++;
			KillsHeavy += (isHeavy) ? 1 : 0;
		}

		public void Save()
		{
			DataManager.SaveData("GameData", this);
		}

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
	}

	public class PermData
	{
		public int[] Gems { get; set; } = new int[3];

		public PermData(bool instantLoad = true)
		{
			GD.Print("New PermData created!");
			for (int i = 0; i < Gems.Length; i++)
				Gems[i] = 0;
			if (instantLoad)
				Load();
		}
		void AddGem(int index)
		{
			Gems[index]++;
		}

		public void Save()
		{
			DataManager.SaveData("PermData", this);
		}

		public void Load()
		{
			var data = DataManager.LoadData<PermData>("PermData");
			if (data == null) { GD.Print("No PermData saved found!, creating empty"); return; }
			Gems = data.Gems;
		}
	}

	public class InventoryData
	{
		public Item[] Items { get; set; }
		public InventoryData(bool instantLoad = true)
		{
			GD.Print("New InventoryData created!");
			Items = new Item[40]; // Example size
			if (instantLoad)
				Load();
		}

		public void AddItem(Item newItem)
		{
			for (int i = 0; i < Items.Length; i++)
			{
				if (Items[i] == null)
				{
					Items[i] = newItem;
					break;
				}
			}
		}

		public void RemoveItem(int index)
		{
			if (index >= 0 && index < Items.Length)
			{
				Items[index] = null;
			}
		}

		public void Save()
		{
			DataManager.SaveData("InventoryData", this);
		}

		public void Load()
		{
			var data = DataManager.LoadData<InventoryData>("InventoryData");
			if (data == null) { GD.Print("No InventoryData saved found!, creating empty"); return; }
			Items = data.Items;
		}
	}

	public class DataManager
	{


		private static string PathOf(string name) => $"user://{name}.json";

		public static T LoadData<T>(string fileName)
		{
			var path = PathOf(fileName);
			if (!FileAccess.FileExists(path)) return default;

			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			string json = f.GetAsText();
			return JsonSerializer.Deserialize<T>(json);
		}

		public static void SaveData<T>(string fileName, T data)
		{
			var path = PathOf(fileName);
			using var f = FileAccess.Open(path, FileAccess.ModeFlags.Write);
			f.StoreString(JsonSerializer.Serialize(data));
		}

		public static void DeleteAllData()
		{
			foreach (var name in new[] { "GameData", "PermData", "InventoryData" })
			{
				var path = PathOf(name);
				if (FileAccess.FileExists(path))
					DirAccess.RemoveAbsolute(path);
			}
		}
	}
}
