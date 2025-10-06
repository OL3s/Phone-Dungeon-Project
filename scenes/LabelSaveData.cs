using Godot;

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
		UpdateLabel();
	}

	public void UpdateLabel()
	{
		var saveNode = GetTree().Root.GetNode("Main").GetNode("SaveData") as SaveData;
		if (saveNode == null)
		{
			GD.PrintErr("No SaveData node found in scene tree in LabelSaveData");
			Text = "NaN";
			return;
		}

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

		var property = dataObject.GetType().GetProperty(ValueName);
		if (property != null)
		{
			var value = property.GetValue(dataObject);
			Text = stringBefore + (value?.ToString() ?? "Null") + stringAfter;
		}
		else
		{
			Text = stringBefore + "Property Not Found" + stringAfter;
		}
	}
}
