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
		animateTarget.scale.X = AnimationLib.LerpXScale(delta, Scale.X, animateTarget.scale.X, 0.1f);
		animateTarget.scale.Y = AnimationLib.LerpYScale(delta, Scale.Y, animateTarget.scale.Y, 0.1f);
		animateTarget.opacity = AnimationLib.LerpAlpha(delta, Modulate.A, animateTarget.opacity, 0.1f);
		animateTarget.rotation = AnimationLib.LerpRotation(delta, RotationDegrees, animateTarget.rotation, 0.1f);

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
			else {
				if (Mathf.Abs(Position.Y) > 0.01f)
					Position = new Vector2(0.0f, AnimationLib.Lerp((float)delta, Position.Y, 0.0f, 20f));
				if (Mathf.Abs(RotationDegrees - animateTarget.rotation) > 0.01f)
					RotationDegrees = AnimationLib.Lerp((float)delta, RotationDegrees, animateTarget.rotation, 20f);
				Timer = 0.0f;
			}
		}
	}
}
