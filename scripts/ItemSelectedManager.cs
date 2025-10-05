using Godot;
using System;
using Items;

public partial class ItemSelectedManager : Control
{

	public Item SelectedItem { get; set; }

	public override void _Ready()
	{
		Visible = false;
		SelectedItem = null;
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
}
