using Godot;
using Animation;

public partial class MobDefaultSprite2D : Sprite2D
{
	public struct AnimateTarget
	{
		public float rotation;
		public Vector2 scale;
		public float opacity;
	}

	public AnimateTarget animateTarget = new AnimateTarget();

	public override void _Ready()
	{
		base._Ready();
		animateTarget.rotation = RotationDegrees;
		animateTarget.scale = Scale;
		animateTarget.opacity = Modulate.A;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		animateTarget.scale.X = AnimationLib.LerpXScale(delta, Scale.X, animateTarget.scale.X, 0.1f);
		animateTarget.scale.Y = AnimationLib.LerpYScale(delta, Scale.Y, animateTarget.scale.Y, 0.1f);
		animateTarget.opacity = AnimationLib.LerpAlpha(delta, Modulate.A, animateTarget.opacity, 0.1f);
		animateTarget.rotation = AnimationLib.LerpRotation(delta, RotationDegrees, animateTarget.rotation, 0.1f);

	}
}
