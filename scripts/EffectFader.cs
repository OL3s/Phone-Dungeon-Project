using Godot;

public partial class EffectFader : Control
{
	[Export] public float Delay = 3f;
	[Export] public float DelayAfter = 6f;
	[Export] public float FadeIn = 4f;
	[Export] public float Visible = 3f;
	[Export] public float FadeOut = 6f;

	private double _start;

	public override void _Ready() => _start = Time.GetTicksMsec() / 1000.0;

	public override void _Process(double delta)
	{
		var t = (float)(Time.GetTicksMsec()/1000.0 - _start);

		float first = Delay + FadeIn + Visible + FadeOut;
		float repeat = DelayAfter + FadeIn + Visible + FadeOut;

		float a;
		if (t < Delay) a = 0f;
		else if (t < first)
		{
			float u = t - Delay;
			if (u < FadeIn) a = u / FadeIn;
			else if (u < FadeIn + Visible) a = 1f;
			else a = 1f - (u - FadeIn - Visible) / FadeOut;
		}
		else
		{
			float v = (t - first) % repeat;
			if (v < DelayAfter) a = 0f;
			else
			{
				float w = v - DelayAfter;
				if (w < FadeIn) a = w / FadeIn;
				else if (w < FadeIn + Visible) a = 1f;
				else a = 1f - (w - FadeIn - Visible) / FadeOut;
			}
		}

		a = Mathf.Clamp(a, 0f, 1f);
		Modulate = new Color(1,1,1,a);

		// midlertidig debug: se faktisk alpha
		GD.Print($"alpha={a:0.00}, t={t:0.00}");
	}
}
