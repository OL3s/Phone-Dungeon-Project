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
		if (IncludeGameData) gameData = new GameData(true);
		if (IncludePermData) permData = new PermData(true);
		if (IncludeInventoryData) inventoryData = new InventoryData(true);
	}

	// Handle data input event
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			// Check for Ctrl+S
			if (keyEvent.CtrlPressed && keyEvent.Keycode == Key.S)
			{
				GD.Print("[SaveData] Ctrl+S pressed - triggering SaveAll()");
				SaveAll();
				GetViewport().SetInputAsHandled(); // Prevent further processing
			}
			// Check for Ctrl+D
			else if (keyEvent.CtrlPressed && keyEvent.Keycode == Key.D)
			{
				GD.Print("[SaveData] Ctrl+D pressed - deleting all data and quitting");
				DeleteAllData();
				GetTree().Quit();
				GetViewport().SetInputAsHandled(); // Prevent further processing
			}
		}
	}

	// Save on exit
	public override void _ExitTree()
	{
		if (SaveOnExit) {
			GD.Print("[SaveData] Node exiting tree. SaveAll() called.");
			SaveAll();
		}
	}

	public override void _Notification(int what)
	{
		if (what == NotificationApplicationPaused)
		{
			// App is going to background
			if (SaveOnExit) 
			{
				GD.Print("[SaveData] Application paused. SaveAll() called.");
				SaveAll();
			}
		}
	}

	// mock shop-inventory for testing

	/// <summary>
	/// Saves all gamedata.
	/// </summary>
	public void SaveAll()
	{
		GD.Print("[SaveData] Saving all data from node.");
		if (IncludeGameData)
			SaveGameData();
		if (IncludePermData)
			SavePermData();
		if (IncludeInventoryData)
			SaveInventoryData();
	}

	/// <summary> Saves game data if included in the configuration. </summary>
	/// <exception cref="InvalidOperationException">Thrown when gameData is null.</exception>
	public void SaveGameData()
	{
		GD.Print("[SaveData] Saving game data from node.");
		if (IncludeGameData && gameData != null)
			gameData.Save();
		else if (IncludeGameData && gameData == null)
			throw new InvalidOperationException("Cannot save gamedata, defined as included but it is null");
	}

	/// <summary> Saves permanent data if included in the configuration.
	/// </summary>
	/// <exception cref="InvalidOperationException">Thrown when permData is null.</exception>
	public void SavePermData()
	{
		GD.Print("[SaveData] Saving permanent data from node.");
		if (IncludePermData && permData != null)
			permData.Save();
		else if (IncludePermData && permData == null)
			throw new InvalidOperationException("Cannot save permdata, defined as included but it is null");
	}

	/// <summary> Saves inventory data if included in the configuration. </summary>
	/// <exception cref="InvalidOperationException">Thrown when inventoryData is null.</exception>
	public void SaveInventoryData()
	{
		GD.Print("[SaveData] Saving inventory data from node.");
		if (IncludeInventoryData && inventoryData != null)
			inventoryData.Save();
		else if (IncludeInventoryData && inventoryData == null)
			throw new InvalidOperationException("Cannot save inventory data, defined as included but it is null");
	}
	
	public void DeleteGameData()
	{
		GD.Print("[SaveData] Deleting GameData file.");
		DataManager.DeleteGameData();
	}
	
	public void DeletePermData()
	{
		GD.Print("[SaveData] Deleting PermData file.");
		DataManager.DeletePermData();
	}
	
	public void DeleteInventoryData()
	{
		GD.Print("[SaveData] Deleting InventoryData file.");
		DataManager.DeleteInventoryData();
	}
	
	public void DeleteAllData() 
	{
		GD.Print("[SaveData] Deleting all data files.");
		DeletePermData();
		DeleteGameData();
		DeleteInventoryData();
	}
	
}
