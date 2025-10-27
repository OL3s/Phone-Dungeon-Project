using Godot;
using System;

// VBoxVertical
public partial class CategoryManager : Control
{
	[Export] public MenuButtons MenuButtonContainer;
	[Export] public Control ContractSelectedMenu;

	public override void _Ready(){
		
		// Make visible
		Visible = true;
		
		// Connect to buttons
		MenuButtonContainer.Connect(
			MenuButtons.SignalName.MenuChangeButtonPressed,
			Callable.From<int>(OnMenuButtonPressed)
		);
		
		// Set current menu
		OnMenuButtonPressed(2);
	}

	// Make only target menu visible
	private void OnMenuButtonPressed(int index) {
		GD.Print($"[CategoryManager] Button index {index} pressed");
		var i = 0;
		
		// Make Ui visible
		foreach(Control ui in GetChildren()) {
			ui.Visible = (i == index);
			i++;
		}
		
		// Make ContractSelectedMenu not visible
		ContractSelectedMenu.Visible = false;
	}
}
