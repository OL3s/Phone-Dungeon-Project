using System;
using Godot;

namespace InputManager
{
	public class InputValues
	{

		public Vector2 Output { get; private set; } = Vector2.Zero; // output vector
		public InputPositionType PositionType { get; private set; } // position type of the input
		private float deadzone { get; } = 0.2f; // deadzone for input
		private float holdTimer { get; set; } = 0.0f; // timer for how long the input has been held
		private float holdThreshold { get; } = 0.5f; // threshold for considering the input as held
		public bool IsTapped { get; private set; } = false; // whether the input was just tapped
		public bool IsDragging { get; private set; } = false; // whether the input is being dragged
		public InputValues() { }
		public InputValues(InputPositionType posType, float deadzone = 0.2f) {
			PositionType = posType;
			this.deadzone = deadzone;
		}

		/// <summary>
		/// Updates the input values based on the provided parameters.
		/// </summary>
		/// <param name="delta">The time elapsed since the last update.</param>
		/// <param name="pressed">Whether the input is currently pressed.</param>
		/// <param name="rawInput">The raw input vector. Length must be between 0 and 1.</param>
		/// <exception cref="ArgumentOutOfRangeException"></exception> 
		public void Update(double delta, bool pressed, Vector2 rawInput)
		{
			// Debug length
			if (rawInput.Length() > 1.0f)
				throw new ArgumentOutOfRangeException("rawInput", "Input vector length cannot exceed 1.0f < " + rawInput.Length() + ">");

			// Hold Timer
			if (pressed)
				holdTimer += (float)delta;
			else
				holdTimer = 0.0f;

			// Check tap & drag
			IsTapped = !pressed && holdTimer > 0.0f && holdTimer < holdThreshold;
			IsDragging = pressed && rawInput.Length() > deadzone;
			Output = IsDragging ? rawInput : Vector2.Zero;

			if (IsTapped)
				GD.Print($"[InputValues] {PositionType} was tapped.");
		}
		
		public override string ToString()
		{
			return $"Output: {Output}, IsDragging: {IsDragging}, IsTapped: {IsTapped}";
		}

		public static InputPositionType GetTouchPositionType(Vector2 touchPos, Vector2 screenSize)
		{
			if (touchPos.Y > screenSize.Y * 0.5f)
			{
				return InputPositionType.ActionTop;
			}
			else if (touchPos.X < screenSize.X * 0.5f)
			{
				return InputPositionType.Movement;
			}
			else
			{
				return InputPositionType.ActionMain;
			}
		}
	}
	
	
	public enum InputPositionType { Movement, ActionMain, ActionTop };
	public enum InputType { Auto, Gamepad, Keyboard, Touch };
}
