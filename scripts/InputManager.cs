using Godot;

namespace InputManager
{
	public class InputValues
	{
		public float MaxRadius = 100f;   // pixels from StartPos for full strength
		public static float DeadZone = 0.15f;  // 0..1 (15%)
		public float TapTime = 0.25f;  // seconds

		public Vector2 StartPos { get; set; }
		public Vector2 CurrentPos { get; set; }
		public InputPositionType? inputPositionType { get; set; }

		private Vector2 Delta => CurrentPos - StartPos;
		private float RawStrength01 => Mathf.Clamp(Delta.Length() / Mathf.Max(1f, MaxRadius), 0f, 1f);

		public float Strength
		{
			get
			{
				float s = RawStrength01;
				if (s <= DeadZone) return 0f;
				return (s - DeadZone) / (1f - DeadZone);
			}
		}

		public Vector2 Direction => Delta == Vector2.Zero ? Vector2.Zero : Delta.Normalized();
		public Vector2 Output => Direction * Strength;

		public double TimeHeld { get; private set; }
		public bool IsDown { get; private set; }
		public bool JustPressed { get; private set; }
		public bool JustReleased { get; private set; }

		public bool IsDrag => Strength > DeadZone && IsDown;
		public bool IsTap => JustReleased && TimeHeld <= TapTime && RawStrength01 < DeadZone;
		public bool IsHold => IsDown && TimeHeld > TapTime && !IsDrag && !IsTap;

		/*
			IsTap is for tap ability
			IsDrag is for drag ability
		*/
		
		public InputValues() { }
		public InputValues(InputPositionType posType) {
			inputPositionType = posType;
		}

		/// <summary>
		/// Update input values
		/// </summary>
		/// <param name="delta"></param>
		/// <param name="pressed">Whether the input is currently pressed</param>
		/// <param name="pos">The current position of the input</param>
		public void Update(double delta, bool pressed, Vector2 pos)
		{
			// JustPressed
			if (pressed && !IsDown)
			{
				JustPressed = true;
				StartPos = pos;
				TimeHeld = 0;
				GD.Print($"[InputValues] JustPressed at {StartPos}");
			}
			else JustPressed = false;

			// JustReleased
			if (!pressed && IsDown) {
				JustReleased = true;
				GD.Print($"[InputValues] JustReleased after {TimeHeld} seconds");
			} else {
				JustReleased = false;
			}

			// Update state
			IsDown = pressed;
			CurrentPos = pos;
			if (IsDown) TimeHeld += delta;
		}

		/*
		public void Reset()
		{
			IsDown = false;
			JustPressed = false;
			JustReleased = false;
			TimeHeld = 0;
			StartPos = Vector2.Zero;
			CurrentPos = Vector2.Zero;
		}
		*/

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
