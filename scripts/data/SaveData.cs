using Godot;
using System;
using System.Text.Json;
using MyClasses;
using MyEnums;

namespace FileData
{
	public class GameData
	{
		public int Gold { get; set; }
		public int Wave { get; set; }
		public Biomes biome { get; set; }
		public int Kills { get; set; }
		public int KillsHeavy { get; set; }

		public GameData()
		{
			Gold = 0;
			Wave = 0;
			biome = Biomes.Forest;
			Kills = 0;
			KillsHeavy = 0;
		}

		void AddGold(int amount)
		{
			Gold += amount;
		}

		void AddWave()
		{
			Wave++;
		}

		void AddKill(bool isHeavy)
		{
			Kills++;
			KillsHeavy += (isHeavy) ? 1 : 0;
		}

		void Save()
		{
			DataManager.SaveData("GameData", this);
		}

		void Load()
		{
			var data = DataManager.LoadData<GameData>("GameData");
			Gold = data.Gold;
			Wave = data.Wave;
			biome = data.biome;
			Kills = data.Kills;
			KillsHeavy = data.KillsHeavy;
		}
	}

	public class PermData
	{
		public int[] Gems { get; set; } = new int[3];

		static void AddGem(int index)
		{
			Gems[index]++;
		}

		static void Save()
		{
			DataManager.SaveData("PermData", this);
		}

		static void Load()
		{
			var data = DataManager.LoadData<PermData>("PermData");
			Gems = data.Gems;
		}
	}

	public class InventoryData
	{
		public Item[] Items { get; set; }

		public InventoryData()
		{
			Items = new Item[40]; // Example size
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
			Items = data.Items;
		}
	}

	public class DataManager
	{


		private static string getPath(string fileName)
		{
			return "user://" + fileName + ".json";
		}

		public static T LoadData<T>(string fileName) where T : new()
		{
			var file = new File();
			T data = new T();
			if (file.FileExists(getPath(fileName)))
			{
				file.Open(getPath(fileName), File.ModeFlags.Read);
				string json = file.GetAsText();
				data = JsonSerializer.Deserialize<T>(json);
				file.Close();
			}
			return data;
		}

		public static void SaveData<T>(string fileName, T data)
		{
			var file = new File();
			file.Open(getPath(fileName), File.ModeFlags.Write);
			string json = JsonSerializer.Serialize(data);
			file.StoreString(json);
			file.Close();
		}

		public static void DeleteAllData()
		{
			var file = new File();
			if (file.FileExists(getPath("GameData")))
			{
				file.Remove(getPath("GameData"));
			}
			if (file.FileExists(getPath("PermData")))
			{
				file.Remove(getPath("PermData"));
			}
			if (file.FileExists(getPath("InventoryData")))
			{
				file.Remove(getPath("InventoryData"));
			}
		}
	}
}
