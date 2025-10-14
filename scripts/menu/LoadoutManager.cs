using Godot;
using Items;

public partial class LoadoutManager : VBoxContainer
{
	[Export] public SaveData saveData;
	[Export] public Control descriptionPanel;
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
				CreateItemNode(item);
			}
		}
	}

	public void RefreshItems()
	{
		GD.Print("[LoadoutManager] Refreshing items in LoadoutManager");
		// Clear existing items
		foreach (Node child in GetChildren())
		{
			if (child is Button button)
				button.QueueFree();
		}
		ProcessItems();
	}

	public void CreateItemNode(Item item)
	{
		var itemTres = GD.Load<PackedScene>("res://assets/prefab/item_button.tscn");
		var itemButton = itemTres.Instantiate<Button>();
		
		// Transparent if item exists in loadout
		//!!TODO!!

		// Get references to the labels
		var labelName = itemButton.GetNode<Label>("LabelName");
		var labelPrice = itemButton.GetNode<Label>("LabelPrice");
		var labelEquipped = itemButton.GetNode<Label>("LabelEquipped");

		// Set the label texts
		labelName.Text = item.Name;
		labelPrice.Text = item.Cost.ToString();
		if (displayMode == "Market")
			labelPrice.Modulate = saveData.gameData.Gold >= item.Cost
				? new Color(1, 1, 1)
				: new Color(1, 0, 0);

			labelEquipped.Text = (displayMode == "Loadout")
				? saveData.gameData.EqualsEquippedIndex(item) switch
				{
					1 => "D",
					2 => "T",
					_ => ""
				}
				: "";

		// Add the button as a child
			AddChild(itemButton);

		// Connect the button press signal
		itemButton.Pressed += () => OnItemButtonPressed(item);
	}

	private void OnItemButtonPressed(Item item)
	{
		// Handle button press logic
		GD.Print($"[LoadoutManager] Button item pressed\n{item}\n");

		// Update description tab with item details
		if (descriptionPanel != null)
		{
			(descriptionPanel as ItemSelectedManager)?.SelectItem(item);
		}
	}
}
