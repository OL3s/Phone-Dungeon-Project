using Godot;
using System;

public partial class GemShowcase : EffectFloat
{
	private bool isVisible;

	public override void _Ready()
	{
		base._Ready();

		// Randomly choose visibility
		isVisible = GD.Randi() % 2 == 0; // 50/50 chance
		Visible = isVisible;
	}
}
