using Godot;
using System;

public partial class EffectFloat : Control
{
	private float currentTime;
	private float yOffset;
	[Export] public float Speed = 300f;
	[Export] public float Amplitude = 5f;
	[Export] public float PhaseOffset = 0f;
	[Export] public float currentTimeOffset = 0f;
	[Export] public float yStart = 0f;

	public override void _Ready()
	{
		currentTime = currentTimeOffset;
	}

	public override void _Process(double delta)
	{
		currentTime += (float)delta * 1000f; // ms if you want
		yOffset = (float)Math.Cos(currentTime / Speed + PhaseOffset) * Amplitude;
		yOffset -= Amplitude;
		Position = new Vector2(Position.X, yStart + yOffset);
	}
}
