using Godot;
using System;
using FileData;

public partial class GemShowcase : Control
{
	[Export] SaveData saveData;
	[Export] int index;
	private Effects.EffectFloat floatEffect;
	private Vector2 originalPosition;

	public override void _Ready()
	{
		// Store original position
		originalPosition = new Vector2(0f, 0f);

		// Initialize the float effect
		floatEffect = new Effects.EffectFloat(new Effects.EffectFloat.Settings(
			false, 			// Is centered
			4f,				// Height
			3f,				// Speed
			index * 0.3f	// Start offset
		));
		
		// Add visible trait
		Visible = saveData.permData.Gems[index] != 0;
	}

	public override void _Process(double delta)
	{
		if (floatEffect != null)
		{
			float yOffset = floatEffect.GetHeight();
			Position = new Vector2(originalPosition.X, originalPosition.Y + yOffset);
		}
	}
}
