using Godot;

public partial class LabelSplashStart : Control
{
	private Effects.EffectFader effectFader;

	public override void _Ready()
	{
		// Called when the node enters the scene tree for the first time.
		// Initialization here
		effectFader = new Effects.EffectFader(new Effects.EffectFader.Settings(
			startVisible: false,
			speed: 0.5f,                    // Speed of fade oscillation
			range: new float[] { -1.0f, 1.0f }, // Alpha range (min, max)
			colorOriginal: Modulate,     // Base color to fade
			startOffset: 0.0f               // Phase offset for variation
		));
	}

	public override void _Process(double delta)
	{
		Modulate = effectFader.GetColor();
	}
}
