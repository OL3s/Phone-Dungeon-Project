using Godot;
using System;
using FileData;

/// <summary>
/// SaveData node class to manage game, permanent, and inventory data saving.
/// </summary>
public partial class SaveData : Node
{
	// Exports
	[Export] public bool IncludeGameData;
	[Export] public bool IncludePermData;
	[Export] public bool IncludeInventoryData;

	// Init datatypes
	public GameData gameData;
	public PermData permData;
	public InventoryData inventoryData;

	// Load on execute
	public override void _Ready()
	{
		if (IncludeGameData) gameData = new GameData();
		if (IncludePermData) permData = new PermData();
		if (IncludeInventoryData) inventoryData = new InventoryData();
	}

	/// <summary>
	/// Saves all gamedata.
	/// </summary>
	public void SaveAll()
	{
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
		else if (gameData == null)
			throw new InvalidOperationException("Cannot save gamedata, it is null");
	}

	/// <summary> Saves permanent data if included in the configuration.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when permData is null.</exception>
	public void SavePermData()
	{
		if (IncludePermData && permData != null)
			permData.Save();
		else if (permData == null)
			throw new InvalidOperationException("Cannot save permdata, it is null");
	}

	/// <summary> Saves inventory data if included in the configuration. </summary>
	/// <exception cref="InvalidOperationException">Thrown when inventoryData is null.</exception>
	public void SaveInventoryData()
	{
		if (IncludeInventoryData && inventoryData != null)
			inventoryData.Save();
		else if (inventoryData == null)
			throw new InvalidOperationException("Cannot save inventory data, it is null");
	}
}
