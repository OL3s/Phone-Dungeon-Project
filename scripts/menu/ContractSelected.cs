using Godot;
using System;
using MyClasses;

public partial class ContractSelected : Control
{
	[Export] BaseButton ButtonBack;
	[Export] BaseButton ButtonStart;
	[Export] Control CategoryContract;
	
	// Show Data
	[Export] TextureRect TextureBiome;
	[Export] Label LabelBiome;
	[Export] Label LabelDifficulty;
	[Export] Label LabelMissionType;
	[Export] TextureRect TextureReward;
	[Export] Label LabelReward;

	public Contract CurrentContract = null;

	public override void _Ready()
	{
		Visible = false;
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
		GD.Print("Start button pressed! TODO -> START MISSION");
		if (CurrentContract == null)
		{
			throw new Exception("No contract selected on start button pressed, cannot start game!");
		}
	}

	public void UpdateContractInfo()
	{

		// Ensure we have a contract to show
		if (CurrentContract == null)
		{
			throw new Exception("No contract selected to update info!");
		}

		// Give correct info to UI elements
		TextureBiome.Texture = Converters.BiomeToTexture(CurrentContract.Biome);
		LabelBiome.Text = CurrentContract.Biome.ToString();
		LabelDifficulty.Text = new string('*', (int)CurrentContract.Difficulty);
		LabelMissionType.Text = CurrentContract.Mission.ToString();
		LabelReward.Text = CurrentContract.Reward.ToString() + "C";
		
		// Make Reward visible only if > 0
		bool isReward = (CurrentContract.Reward > 0);
		LabelReward.Visible = isReward;
		TextureReward.Visible = isReward;
		
	}

}
