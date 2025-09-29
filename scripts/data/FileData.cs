using Godot;
using System;
using System.Text.Json;
using MyClasses;
using MyEnums;

namespace FileData
{
	public class GameData
	{
		public int Gold { get; set; } = 0;
		public int Wave { get; set; } = 0;
		public Biomes Biome { get; set; } = Biomes.Woodland;
		public int Kills { get; set; } = 0;
		public int KillsHeavy { get; set; } = 0;

		public GameData(bool instantLoad = true)
		{
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
		public int[] Gems { get; set; } = { 0, 0, 0 };

		public PermData(bool instantLoad = true)
		{
			GD.Print("PermData Init");
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
		public Item[] Items { get; set; } = new Item[40];
		public InventoryData(bool instantLoad = true)
		{
			GD.Print("InventoryData Init");
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

	public static class DataManager
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
