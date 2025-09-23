using Godot;
using System;
using MyClasses;

public partial class GameData : Node
{
	[Export] public bool IncludeGlobalData;
	[Export] public bool IncludeGameData;
	[Export] public bool IncludeInventoryData;

	private Godot.Collections.Dictionary fileGlobal;
	private Godot.Collections.Dictionary fileGame;
	private Godot.Collections.Dictionary fileInventory;

	public override void _Ready()
	{
		if (IncludeGlobalData)
			fileGlobal = LoadJsonFile("user://global_data.json");

		if (IncludeGameData)
			fileGame = LoadJsonFile("user://game_data.json");

		if (IncludeInventoryData)
			fileInventory = LoadJsonFile("user://inventory_data.json");
	}

	private Godot.Collections.Dictionary LoadJsonFile(string path)
	{
		if (!FileAccess.FileExists(path))
			return new Godot.Collections.Dictionary(); // Return empty if not found

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string jsonText = file.GetAsText();
		var result = Json.ParseString(jsonText);

		if (result.VariantType == Variant.Type.Dictionary)
			return (Godot.Collections.Dictionary)result;

		return new Godot.Collections.Dictionary();
	}

	public void SaveJsonFile(string path, Godot.Collections.Dictionary data)
	{
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		string jsonText = Json.Stringify(data);
		file.StoreString(jsonText);
	}
}
