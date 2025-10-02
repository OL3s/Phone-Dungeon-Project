using Godot;
using System;
using MyClasses;
using FileData;
using Effects;

public partial class ContractButton : Control
{
	[Export] Control ContractSelectedMenu;
	[Export] Control ContractMenu;
	[Export] int Index;
	[Export] SaveData saveData;
	
	private GameData gameData;
	public Contract contract;
	
	public override void _Ready()
	{
		gameData = saveData.gameData;
		contract = new Contract(gameData.Biome, gameData.Wave, gameData.ContractSeed + Index);

		// Get the categories control
		if (ContractSelectedMenu == null || ContractMenu == null)
		{
			throw new Exception("ContractSelectedMenu | ContractMenu not assigned in ContractButton");
		}
		
		// Update biome image
		GetNode("Button").GetNode<TextureRect>("BiomeTexture").Texture = Converters.BiomeToTexture(contract.Biome);

		// Update difficulty label
		GetNode("Button").GetNode<Label>("DifficultyLabel").Text = new string('*', contract.Difficulty);

		// Connect the button pressed signal
		GetNode<Button>("Button").Pressed += OnButtonPressed;
	}

	public override void _Process(double delta)
	{
		// Float animation
		Position = new Vector2(Position.X, 6 * (float)Math.Sin(Time.GetTicksMsec() / 500.0 + Index));
	}

	private void OnButtonPressed()
	{
		GD.Print("Contract pressed! -> TODO ADD: GOTO MISSION");
		ContractSelectedMenu.Visible = true;
		
		// Set contract for selected menu
		((ContractSelected)ContractSelectedMenu).CurrentContract = contract;
		
		ContractMenu.Visible = false;
	}
}
