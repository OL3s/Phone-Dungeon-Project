using Godot;
using System;
using MyClasses;
using FileData;
using MyEnums;

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
		contract = new Contract(gameData.Biome, gameData.Wave, gameData.Seed + Index);

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

		// Set contract type
		GetNode("Button").GetNode<Label>("MissionLabel").Text = contract.Mission.ToString();

		// Make contract invisible if no mission
		GetNode("Button").GetNode<Label>("MissionLabel").Visible 		 = contract.Mission != MissionType.None;
		GetNode("Button").GetNode<TextureRect>("MissionTexture").Visible = contract.Mission != MissionType.None;
	}

	public override void _Process(double delta)
	{
		// Float animation
		Position = new Vector2(Position.X, 6 * (float)Math.Sin(Time.GetTicksMsec() / 500.0 + Index));
	}

	private void OnButtonPressed()
	{
		GD.Print("[ContractButton] Contract pressed!");
		ContractSelectedMenu.Visible = true;
		ContractMenu.Visible = false;
		
		// Set contract for selected menu + update
		((ContractSelected)ContractSelectedMenu).CurrentContract = contract;
		((ContractSelected)ContractSelectedMenu).UpdateContractInfo();
		
	}
}
