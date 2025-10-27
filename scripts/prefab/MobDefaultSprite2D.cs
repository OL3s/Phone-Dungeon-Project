using Godot;
using Animation;

public partial class MobDefaultSprite2D : Sprite2D
{
	[Export] public CharacterBody2D ParentBody;
	public struct AnimateTarget
	{
		public float rotation;
		public Vector2 scale;
		public float opacity;
		void reset() 
		{
			rotation = 0.0f;
			scale = Vector2.One;
			opacity = 1.0f;
		}
	}

	public AnimateTarget animateTarget = new AnimateTarget();
	private float Timer = 0.0f;

	public override void _Ready()
	{
		base._Ready();
		animateTarget.rotation = RotationDegrees;
		animateTarget.scale = Scale;
		animateTarget.opacity = Modulate.A;

		if (ParentBody == null)
			GD.PrintErr("[MobDefaultSprite2D] ParentBody is not assigned!");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		// Lerp
		Scale = new Vector2(
			AnimationLib.LerpXScale(delta, Scale.X, animateTarget.scale.X, 1f),
			AnimationLib.LerpYScale(delta, Scale.Y, animateTarget.scale.Y, 1f)
		);
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B,
			AnimationLib.LerpAlpha(delta, Modulate.A, animateTarget.opacity, 10f)
		);
		RotationDegrees = AnimationLib.LerpRotation(delta, RotationDegrees, animateTarget.rotation, 10f);

		// Apply lerped values
		Scale = animateTarget.scale;
		Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, animateTarget.opacity);

		if (ParentBody != null)
		{
			float speed = ParentBody.Velocity.Length();
			if (speed > 0.1f)
			{
				// Bounce effect when moving
				Timer += (float)delta;
				Position = new Vector2(
					0.0f,
					-AnimationLib.BounceMovementOffset(Timer, 3.0f, 10.0f)
				);
				// Simple swing back and forth
				RotationDegrees = AnimationLib.SwayMovementOffset(Timer, 10.0f, 10.0f);
			}
			else
			{
				if (Mathf.Abs(Position.Y) > 0.01f)
					Position = new Vector2(0.0f, AnimationLib.Lerp((float)delta, Position.Y, 0.0f, 20f));

				if (Mathf.Abs(RotationDegrees - animateTarget.rotation) > 0.01f)
					RotationDegrees = AnimationLib.Lerp((float)delta, RotationDegrees, animateTarget.rotation, 20f);

				Timer = 0.0f;
			}

			// set direction
			if (ParentBody.Velocity.X < -0.1f)
				animateTarget.scale.X = -1;
			else if (ParentBody.Velocity.X > 0.1f)
				animateTarget.scale.X = 1;
		}
	}
}
