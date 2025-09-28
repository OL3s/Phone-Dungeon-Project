using Godot;
using System;

public partial class LabelSaveData : Label
{
	[Export(PropertyHint.Enum, "GameData,PermData,InventoryData,SettingsData")]
	string SaveDataName = "";

	[Export]
	string ValueName = "";

	public override void _Ready()
	{
		var saveNode = GetTree().Root.GetNode("Main").GetNode("GameData") as SaveData;
		if (saveNode == null) Text = "NaN";
		else Text = saveNode.gameData.Gold.ToString();
	}
}
