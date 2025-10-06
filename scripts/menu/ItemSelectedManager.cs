using Godot;
using System;
using Items;

public partial class ItemSelectedManager : Control
{

	[Export] public Button ButtonBuy;
	[Export] public Button ButtonEquip;
	[Export] public Button ButtonEquipTop;
	[Export] public SaveData saveData;
	[Export] public LoadoutManager ListItemsMarket;
	[Export] public LoadoutManager ListItemsLoadout;
	[Export] public LabelSaveData HudGoldLabel;
	[Export (PropertyHint.Enum, "Market,Loadout")] public string currentList = "Market";

	public Item SelectedItem { get; set; }
	public Control SelectedItemObject { get; set; }

	public override void _Ready()
	{
		Visible = false;
		SelectedItem = null;

		// Debugger for missing SaveData node
		if (saveData == null)
		{
			GD.PrintErr("No SaveData node assigned in ItemSelectedManager");
			return;
		}

		// Connect button signals
		if (ButtonBuy != null) ButtonBuy.Pressed += PressedBuy;
		if (ButtonEquip != null) ButtonEquip.Pressed += PressedEquipItem;
		if (ButtonEquipTop != null) ButtonEquipTop.Pressed += PressedEquipItemTop;
	}

	public void SelectItem(Item item)
	{
		if (item == null)
		{
			GD.Print("'No item' selected in ItemSelectedManager");
			Visible = false;
			SelectedItem = null;
			return;
		}

		GD.Print($"Selected item:\n{item}\nin ItemSelectedManager");
		SelectedItem = item; // add selected item

		// enable buy button based on price
		if (ButtonBuy != null)
			ButtonBuy.Visible = (item.Cost <= saveData.gameData.Gold) ? true : false;

		// cost label
		Label costLabel = GetNode<Label>("CostLabel");
		costLabel.Text = "C " + item.Cost.ToString();
		costLabel.Modulate = (currentList == "Loadout" || item.Cost <= saveData.gameData.Gold)
			? new Color(1, 1, 1)
			: new Color(1, 0, 0);

		// name label
		GetNode<Label>("NameLabel").Text = item.Name;

		// make visible5
		Visible = true;


	}

	public void PressedBuy()
	{
		GD.Print("Pressed buy item");

		// ========= Debuggers =========
		if (SelectedItem == null) throw new Exception("No item selected to buy");
		if (saveData == null) throw new Exception("No SaveData assigned in ItemSelectedManager");
		if (SelectedItem.Cost > saveData.gameData.Gold) throw new Exception("Tried to buy an item without enough gold!");
		if (SelectedItem.Index == null) throw new Exception("Cannot buy item which has no MarketItem index");
		if (HudGoldLabel == null) GD.PrintErr("No HudGoldLabel assigned in ItemSelectedManager");
		// =============================

		// Preserve the market index BEFORE AddItem mutates Item.Index
		int marketIndex = SelectedItem.Index.Value;

		/* 1. Lose Gold */
		saveData.gameData.Gold -= SelectedItem.Cost;

		/* 2. Add in inventory */
		int inventoryIndex = saveData.inventoryData.AddItem(SelectedItem);
		if (inventoryIndex == -1) throw new Exception("No space in inventory to add item");

		/* 3. Remove from market (use preserved index) */
		// saveData.gameData.MarketItems[SelectedItem.Index.Value] = null; // wrong after AddItem
		saveData.gameData.RemoveMarketItem(marketIndex);

		/* 4. Save changes */
		saveData.SaveAll();

		// Update HUD label text
		HudGoldLabel?.UpdateLabel();

		/* 5. Refresh lists */
		ListItemsMarket?.RefreshItems();
		ListItemsLoadout?.RefreshItems();

		Visible = false;
		SelectedItem = null;
	}

	public void PressedEquipItem()
	{
		saveData.gameData.SetEquipped(SelectedItem, false);
		SelectItem(SelectedItem);
		ListItemsLoadout?.RefreshItems();
		ListItemsMarket?.RefreshItems();
	}

	public void PressedEquipItemTop()
	{
		saveData.gameData.SetEquipped(SelectedItem, true);
		SelectItem(SelectedItem);
		ListItemsLoadout?.RefreshItems();
		ListItemsMarket?.RefreshItems();
	}
}
