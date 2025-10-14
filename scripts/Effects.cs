using Godot;


namespace Effects
{

	/* USAGE:

	// EffectFloat - Creates floating animation effect
	var floatSettings = new EffectFloat.Settings(
		isCentered: true,     // Center the oscillation around 0
		height: 15.0f,        // Amplitude of the float effect
		speed: 2.0f,          // Speed of oscillation
		startOffset: 0.5f     // Phase offset for variation
	);
	var floatEffect = new EffectFloat(floatSettings);
	
	// In your _Process or _PhysicsProcess method:
	Position = basePosition + Vector2.Up * floatEffect.GetHeight();

	// EffectFader - Creates color fading animation effect
	var fadeSettings = new EffectFader.Settings(
		speed: 1.5f,                    // Speed of fade oscillation
		range: new float[] {0.3f, 1.0f}, // Alpha range (min, max)
		colorOriginal: Colors.White,     // Base color to fade
		startOffset: 1.0f               // Phase offset for variation
	);
	var fadeEffect = new EffectFader(fadeSettings);
	
	// In your _Process method:
	Modulate = fadeEffect.GetColor();

	*/

	public class EffectFloat
	{
		public struct Settings
		{
			public bool IsCentered;
			public float Height;
			public float Speed;
			public float StartOffset;

			public Settings(bool isCentered = false, float height = 10.0f, float speed = 1.0f, float startOffset = 0.0f)
			{
				IsCentered = isCentered;
				Height = height;
				Speed = speed;
				StartOffset = startOffset;
			}
		}

		public Settings EffectSettings { get; set; }

		public EffectFloat(Settings settings)
		{
			EffectSettings = settings;
		}

		public float GetHeight()
		{
			float globalTime = Time.GetTicksMsec() / 1000.0f + EffectSettings.StartOffset;
			float offset = Mathf.Sin(globalTime * EffectSettings.Speed) * EffectSettings.Height;
			return EffectSettings.IsCentered ? offset : offset - EffectSettings.Height;
		}
	}

	public class EffectFader
	{
		public struct Settings
		{
			public bool StartVisible;
			public float Speed;
			public float[] Range; 
			public Color ColorOriginal;
			public float StartOffset;

			public Settings(float speed, float[] range, Color colorOriginal, float startOffset = 0.0f, bool startVisible = true)
			{
				StartVisible = startVisible;
				Speed = speed;
				Range = range;
				ColorOriginal = colorOriginal;
				StartOffset = startOffset;
			}
		}

		public Settings EffectSettings { get; set; }

		public EffectFader(Settings settings)
		{
			EffectSettings = settings;
		}

		/// <summary>
		/// Get the current faded color based on global timer and settings
		/// </summary>
		/// <returns>The interpolated color</returns>
		public Color GetColor()
		{
			if (EffectSettings.Range.Length < 2) return EffectSettings.ColorOriginal;

			float globalTime = Time.GetTicksMsec() / 1000.0f + EffectSettings.StartOffset;
			if (!EffectSettings.StartVisible)
			{
				globalTime += Mathf.Pi / EffectSettings.Speed; // Offset by half a cycle to start invisible
			}
			float progress = (Mathf.Sin(globalTime * EffectSettings.Speed) + 1.0f) * 0.5f;
			float alpha = Mathf.Lerp(EffectSettings.Range[0], EffectSettings.Range[1], progress);
			
			var color = EffectSettings.ColorOriginal;
			color.A = alpha;
			return color;
		}
	}
}
