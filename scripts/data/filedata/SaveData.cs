using Godot;
using System;
using FileData;
using Items;
using Combat;
using System.Formats.Asn1;

/// <summary>
/// SaveData node class to manage game, permanent, and inventory data saving.
/// </summary>
public partial class SaveData : Node
{
	// Exports
	[Export] public bool IncludeGameData;
	[Export] public bool IncludePermData;
	[Export] public bool IncludeInventoryData;
	[Export] public bool SaveOnExit = true;

	// Init datatypes
	public GameData gameData;
	public PermData permData;
	public InventoryData inventoryData;

	// Load on execute
	public override void _Ready()
	{
		if (IncludeGameData) gameData = new GameData();
		if (IncludePermData) permData = new PermData();
		if (IncludeInventoryData)
		{
			inventoryData = new InventoryData();

			// mock inventory for testing
			inventoryData.AddItem(new Item(null, "Sword", 10));

			// mock inventory store weapons for testing
			gameData.AddMarketItem(new Weapon(null, "Sword", 100, new MeleeAttack(1.0f, 1.0f, null, null), 100));
			gameData.AddMarketItem(new Weapon(null, "Bow", 200, new RangedAttack(10.0f, 50.0f, null, null), 100));
			gameData.AddMarketItem(new Weapon(null, "Staff", 150, new BeamAttack(5.0f, 30.0f, 0.0f, null, null), 100));
			gameData.AddMarketItem(new Weapon(null, "Dagger", 50, new MeleeAttack(0.5f, 0.5f, null, null), 100));

		}
	}

	// Save on exit
	public override void _ExitTree()
	{
		if (SaveOnExit) SaveAll();
	}

	public override void _Notification(int what)
	{
		if (what == NotificationApplicationPaused)
		{
			// App is going to background
			if (SaveOnExit) SaveAll();
		}
	}

	// mock shop-inventory for testing

	/// <summary>
	/// Saves all gamedata.
	/// </summary>
	public void SaveAll()
	{
		GD.Print("Saving all before exit in SaveData node");
		SaveGameData();
		SavePermData();
		SaveInventoryData();
	}

	/// <summary> Saves game data if included in the configuration. </summary>
	/// <exception cref="InvalidOperationException">Thrown when gameData is null.</exception>
	public void SaveGameData()
	{
		if (IncludeGameData && gameData != null)
			gameData.Save();
		else if (IncludeGameData && gameData == null)
			throw new InvalidOperationException("Cannot save gamedata, it is null");
	}

	/// <summary> Saves permanent data if included in the configuration.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when permData is null.</exception>
	public void SavePermData()
	{
		if (IncludePermData && permData != null)
			permData.Save();
		else if (IncludePermData && permData == null)
			throw new InvalidOperationException("Cannot save permdata, it is null");
	}

	/// <summary> Saves inventory data if included in the configuration. </summary>
	/// <exception cref="InvalidOperationException">Thrown when inventoryData is null.</exception>
	public void SaveInventoryData()
	{
		if (IncludeInventoryData && inventoryData != null)
			inventoryData.Save();
		else if (IncludeInventoryData && inventoryData == null)
			throw new InvalidOperationException("Cannot save inventory data, it is null");
	}
	
	public void DeleteGameData()
	{
		DataManager.DeleteGameData();
	}
	
	public void DeletePermData()
	{
		DataManager.DeletePermData();
	}
	
	public void DeleteInventoryData()
	{
		DataManager.DeleteInventoryData();
	}
	
	public void DeleteAllData() 
	{
		GD.Print("Deleting All Data");
		DeletePermData();
		DeleteGameData();
		DeleteInventoryData();
	}
	
}
