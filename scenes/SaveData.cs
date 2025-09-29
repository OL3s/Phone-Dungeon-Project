using Godot;
using System;
using FileData;

public partial class SaveData : Node
{
	// Exports
	[Export] public bool IncludeGameData;
	[Export] public bool IncludePermData;
	[Export] public bool IncludeInventoryData;
	
	// Init datatypes
	public FileData.GameData gameData;
	public FileData.PermData permData;
	public FileData.InventoryData inventoryData;

	// Load on execute
	public override void _Ready()
	{
		if (IncludeGameData) gameData = new GameData();
		if (IncludePermData) permData = new PermData();
		if (IncludeInventoryData) inventoryData = new InventoryData();
	}
	
	// Save all function
	public void SaveAll()
	{
		if (IncludeGameData && gameData != null)
			gameData.Save();

		if (IncludePermData && permData != null)
			permData.Save();

		if (IncludeInventoryData && inventoryData != null)
			inventoryData.Save();
	}
}
