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
		
		GD.Print($"Selected item: {item.Name} in ItemSelectedManager");
		SelectedItem = item;
		Visible = true;

		GetNode<Label>("NameLabel").Text = item.Name;
	}
	
	public void PressedBuy() 
	{
		GD.Print("Pressed buy item");
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
