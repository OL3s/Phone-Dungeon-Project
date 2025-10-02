using Godot;
using System;
using MyClasses;

public partial class ContractSelected : Control
{
	[Export] BaseButton ButtonBack;
	[Export] BaseButton ButtonStart;
	[Export] Control CategoryContract;

	public Contract CurrentContract = null;

	public override void _Ready()
	{
		if (ButtonBack == null || ButtonStart == null)
		{
			throw new Exception("Buttons or/and CategoryContract not assigned in ContractSelected");
		}

		ButtonBack.Pressed += OnButtonBackPressed;
		ButtonStart.Pressed += OnButtonStartPressed;
	}

	private void OnButtonBackPressed()
	{
		CategoryContract.Visible = true;
		this.Visible = false;
		GD.Print("Back button pressed!");
	}

	private void OnButtonStartPressed()
	{
		GD.Print("Start button pressed!");
		if (CurrentContract == null)
		{
			throw new Exception("No contract selected on start button pressed, cannot start game!");
		}
		
		
	}

}
