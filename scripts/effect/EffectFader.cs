using Godot;

public partial class EffectFader : Control
{
	[Export] public float Delay = 3f;
	[Export] public float DelayAfter = 6f;
	[Export] public float FadeIn = 4f;
	[Export] public float Visiblee = 3f;
	[Export] public float FadeOut = 6f;

	private double _start;

	public override void _Ready() => _start = Time.GetTicksMsec() / 1000.0;

	public override void _Process(double delta)
	{
		var t = (float)(Time.GetTicksMsec()/1000.0 - _start);

		float first = Delay + FadeIn + Visiblee + FadeOut;
		float repeat = DelayAfter + FadeIn + Visiblee + FadeOut;

		float a;
		if (t < Delay) a = 0f;
		else if (t < first)
		{
			float u = t - Delay;
			if (u < FadeIn) a = u / FadeIn;
			else if (u < FadeIn + Visiblee) a = 1f;
			else a = 1f - (u - FadeIn - Visiblee) / FadeOut;
		}
		else
		{
			float v = (t - first) % repeat;
			if (v < DelayAfter) a = 0f;
			else
			{
				float w = v - DelayAfter;
				if (w < FadeIn) a = w / FadeIn;
				else if (w < FadeIn + Visiblee) a = 1f;
				else a = 1f - (w - FadeIn - Visiblee) / FadeOut;
			}
		}

		a = Mathf.Clamp(a, 0f, 1f);
		Modulate = new Color(1,1,1,a);

		// midlertidig debug: se faktisk alpha
		// GD.Print($"alpha={a:0.00}, t={t:0.00}");
	}
}
