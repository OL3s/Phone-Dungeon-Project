using Godot;
using System;
using System.Reflection;
using FileData;

public partial class LabelSaveData : Label
{
	[Export(PropertyHint.Enum, "gameData,permData,inventoryData")]
	string SaveDataName = "gameData";
	[Export] string stringBefore;
	[Export] string stringAfter;

	[Export]
	string ValueName = "Gold";

	public override void _Ready()
	{
		var saveNode = GetTree().Root.GetNode("Main").GetNode("SaveData") as SaveData;
		if (saveNode == null) 
		{
			Text = "NaN";
			return;
		}

		// Get the correct data object based on SaveDataName
		object dataObject = SaveDataName switch
		{
			"gameData" => saveNode.gameData,
			"permData" => saveNode.permData,
			"inventoryData" => saveNode.inventoryData,
			_ => null
		};

		if (dataObject == null)
		{
			Text = "No Data";
			return;
		}

		// Use reflection to get the property value
		var property = dataObject.GetType().GetProperty(ValueName);
		if (property != null)
		{
			var value = property.GetValue(dataObject);
			Text = value?.ToString() ?? "Null";
		}
		else
		{
			Text = "Property Not Found";
		}
		
		Text = stringBefore + Text + stringAfter;
	}
}
