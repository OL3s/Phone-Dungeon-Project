using System;
using Godot;

namespace Animation
{
	public static class AnimationLib
	{
		public static float SwayMovementOffset(double delta, float range, float speed)
		{
			return (float)(Math.Sin(delta * speed) * range);
		}

		public static float BounceMovementOffset(double delta, float range, float speed)
		{
			// Implement bounce movement calculation
			return (float)(Math.Abs(Math.Sin(delta * speed)) * range);
		}

		public static float LinearMovementOffset(double delta, float range, float speed)
		{
			// Implement linear movement calculation
			return (float)((delta * speed) % range);
		}

		public static float LerpXScale(double delta, float current, float target, float speed)
		{
			// Implement linear interpolation for X scale
			if (Math.Abs(target - current) < 0.01f)
				return target;
			return current + (target - current) * speed * (float)delta;
		}

		public static float LerpYScale(double delta, float current, float target, float speed)
		{
			// Implement linear interpolation for Y scale
			if (Math.Abs(target - current) < 0.01f)
				return target;
			return current + (target - current) * speed * (float)delta;
		}

		public static float LerpAlpha(double delta, float current, float target, float speed)
		{
			// Implement linear interpolation for alpha
			if (Math.Abs(target - current) < 0.01f)
				return target;
			return current + (target - current) * speed * (float)delta;
		}

		public static float LerpRotation(double delta, float current, float target, float speed)
		{
			// Implement linear interpolation for rotation
			if (Math.Abs(target - current) < 0.01f)
				return target;
			return current + (target - current) * speed * (float)delta;
		}

		public static float LerpPositionX(double delta, float current, float target, float speed)
		{
			// Implement linear interpolation for position X
			if (Math.Abs(target - current) < 0.01f)
				return target;
			return current + (target - current) * speed * (float)delta;
		}

		public static float LerpPositionY(double delta, float current, float target, float speed)
		{
			// Implement linear interpolation for position Y
			if (Math.Abs(target - current) < 0.01f)
				return target;
			return current + (target - current) * speed * (float)delta;
		}

		public static float Lerp(double delta, float current, float target, float speed)
		{
			// General linear interpolation
			if (Math.Abs(target - current) < 0.01f)
				return target;
			return current + (target - current) * speed * (float)delta;
		}
	}

	public static class ColorUtils
	{
		public enum ColorDefault
		{
			Red,
			Green,
			Blue,
			Yellow,
			Pink,
			Purple,
			Black,
			White,
			Orange,
			Cyan,
			Magenta,
			Brown,
			Gray,
			Lime,
			Navy,
			Teal,
			Olive,
			Maroon,
			Silver,
			Gold
		}
		public static Color GetColor(ColorDefault color)
		{
			return color switch
			{
				ColorDefault.Red => new Color(1f, 0f, 0f),
				ColorDefault.Green => new Color(0f, 1f, 0f),
				ColorDefault.Blue => new Color(0f, 0f, 1f),
				ColorDefault.Yellow => new Color(1f, 1f, 0f),
				ColorDefault.Pink => new Color(1f, 192f / 255f, 203f / 255f),
				ColorDefault.Purple => new Color(128f / 255f, 0f, 128f / 255f),
				ColorDefault.Black => new Color(0f, 0f, 0f),
				ColorDefault.White => new Color(1f, 1f, 1f),
				ColorDefault.Orange => new Color(1f, 165f / 255f, 0f),
				ColorDefault.Cyan => new Color(0f, 1f, 1f),
				ColorDefault.Magenta => new Color(1f, 0f, 1f),
				ColorDefault.Brown => new Color(165f / 255f, 42f / 255f, 42f / 255f),
				ColorDefault.Gray => new Color(128f / 255f, 128f / 255f, 128f / 255f),
				ColorDefault.Lime => new Color(0f, 1f, 0f),
				ColorDefault.Navy => new Color(0f, 0f, 128f / 255f),
				ColorDefault.Teal => new Color(0f, 128f / 255f, 128f / 255f),
				ColorDefault.Olive => new Color(128f / 255f, 128f / 255f, 0f),
				ColorDefault.Maroon => new Color(128f / 255f, 0f, 0f),
				ColorDefault.Silver => new Color(192f / 255f, 192f / 255f, 192f / 255f),
				ColorDefault.Gold => new Color(1f, 215f / 255f, 0f),
				_ => new Color(1f, 1f, 1f)
			};
		}
	}
}
