using Godot;
using System;
using Items;

public partial class ItemSelectedManager : Control
{

	[Export] public Button ButtonBuy;
	[Export] public Button ButtonEquip;
	[Export] public Button ButtonEquipTop;
	[Export] public SaveData saveData;
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
		if (ButtonEquip != null) ButtonEquip.Pressed += PressedEquip;
		if (ButtonEquipTop != null) ButtonEquipTop.Pressed += PressedEquipTop;
	}

	public void SelectItem(Item item)
	{
		if (item == null) {
			GD.Print("'No item' selected in ItemSelectedManager");
			Visible = false;
			SelectedItem = null;
			return;
		}
		
		GD.Print($"Selected item: {item.Name}[{item.Index ?? null}] in ItemSelectedManager");
		SelectedItem = item; // add selected item
		
		// enable buy button based on price
		if (ButtonBuy != null)
			ButtonBuy.Visible = (item.Cost < saveData.gameData.Gold);
		
		// cost label
		Label costLabel = GetNode<Label>("CostLabel");
		costLabel.Text = "C " + item.Cost.ToString();
		costLabel.Modulate = (ButtonBuy == null || item.Cost < saveData.gameData.Gold)
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
		if (SelectedItem.Cost < saveData.gameData.Gold)
			throw new Exception("Tried to buy an item without enough gold!");
		else {
			if (SelectedItem.Index == null) {
				throw new Exception("Cannot buy item which has no MarketItem index");
			}
			
		/* 1. Loose Gold */				saveData.gameData.Gold -= SelectedItem.Cost; 
		/* 2. Add in inventory */		saveData.inventoryData.AddItem(SelectedItem);
		/* 3. Remove from save */		saveData.gameData.MarketItems[SelectedItem.Index ?? -1] = null;
		/* 4. Save changes */			saveData.SaveAll();
		
		/* 5. Remove market object */
		// ...	
		/* 6. Add inventory object */
		// ...
		}
	}
	
	public void PressedEquip() 
	{
		GD.Print("Pressed equip");
	}
	
	public void PressedEquipTop()
	{
		GD.Print("Pressed equip top");
	}
}
