using Godot;
using System;
using FileData;


public partial class SaveData : Node
{
	[Export] public bool IncludeGlobalData;
	[Export] public bool IncludeGameData;
	[Export] public bool IncludeInventoryData;

	private FileData.Storage _data;

	public FileData.GameData gameData => _data.GameData;
	public FileData.PermData permData => _data.PermData;
	public FileData.InventoryData inventoryData => _data.InventoryData;

	public override void _Ready()
	{
		_data = new FileData.Storage(IncludeGameData, IncludeGlobalData, IncludeInventoryData);
	}
	
	public void SaveAll()
	{
		if (_data == null)
		{
			GD.Print("No data in storage found, nothing to save.");
			return;
		}

		if (IncludeGameData && _data.GameData is not null)
			_data.GameData.Save();

		if (IncludeGlobalData && _data.PermData is not null)
			_data.PermData.Save();

		if (IncludeInventoryData && _data.InventoryData is not null)
			_data.InventoryData.Save();
	}
}
