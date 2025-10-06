using Godot;
using Items;
using System;

public partial class LoadoutManager : VBoxContainer
{
	[Export] public SaveData saveData;
	[Export] public Control descriptionTab;
	[Export(PropertyHint.Enum, "Loadout,Market")] public string displayMode = "Loadout";

	public override void _Ready()
	{
		ProcessItems();
	}

	public void ProcessItems()
	{
		Item[] selectedList = (displayMode == "Loadout")
			? saveData.inventoryData.Items
			: saveData.gameData.MarketItems;
		for (int i = 0; i < selectedList.Length; i++)
		{
			Item item = selectedList[i];
			if (item != null)
			{
				CreateItemNode(item, i);
			}
		}
	}	

	public void CreateItemNode(Item item, int index)
	{
		var itemTres = GD.Load<PackedScene>("res://assets/prefab/item_button.tscn");
		var itemButton = itemTres.Instantiate<Button>();

		// Get references to the labels
		var labelName = itemButton.GetNode<Label>("LabelName");
		var labelPrice = itemButton.GetNode<Label>("LabelPrice");

		// Set the label texts
		labelName.Text = item.Name;
		labelPrice.Text = item.Cost.ToString();

		// Store the index in metadata
		itemButton.SetMeta("Index", index);

		// Add the button as a child
		AddChild(itemButton);

		// Connect the button press signal
		itemButton.Pressed += () => OnItemButtonPressed(index, item);
	}

	private void OnItemButtonPressed(int index, Item item)
	{
		// Handle button press logic
		GD.Print($"Button pressed: {item.Name} at index {index}");

		// Update description tab with item details
		if (descriptionTab != null)
		{
			(descriptionTab as ItemSelectedManager)?.SelectItem(item);
		}
	}
}
