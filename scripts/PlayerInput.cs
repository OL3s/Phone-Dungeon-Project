using Godot;
using System;
using InputManager;

public partial class PlayerInput : Node
{
	[Export] public InputType inputType { get; set; } = InputType.Auto;

	public InputValues[] inputs = new InputValues[Enum.GetValues<InputPositionType>().Length];
	
	
	// Touch tracking
	public override void _Ready()
	{
		// Create "Joysticks"
		for (int i = 0; i < inputs.Length; i++)
		{
			inputs[i] = new InputValues((InputPositionType)i);
			GD.Print($"[PlayerInput] Added InputValues for {(InputPositionType)i}");
		}

		// Find input type if auto
		if (inputType == InputType.Auto)
		{
			var joypads = Input.GetConnectedJoypads();
			string platform = OS.GetName();

			if (platform == "Android" || platform == "iOS")
				inputType = InputType.Touch;
			else if (joypads.Count > 0)
				inputType = InputType.Gamepad;
			else
				inputType = InputType.Keyboard;
		}

		GD.Print($"[PlayerInput] Using input type: {inputType}");
	}

	public override void _Process(double delta)
	{
		// Update inputs based on input type
		switch (inputType)
		{
			case InputType.Touch:
				return;
			case InputType.Gamepad:
				UpdateGamepadInputs(delta);
				break;
			case InputType.Keyboard:
				UpdateKeyboardInputs(delta);
				break;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (inputType != InputType.Touch)
			return;

		// !! TODO Handle raw input events here
	}

	private void UpdateGamepadInputs(double delta)
	{
		// Movement
		var movementInput = inputs[(int)InputPositionType.Movement];
		Vector2 rawMovementVector = new Vector2(
			Input.GetJoyAxis(0, JoyAxis.LeftX),
			Input.GetJoyAxis(0, JoyAxis.LeftY)
		);

		// Update with error handling
		try 
		{
			movementInput.Update(delta, true, rawMovementVector);
		}
		catch 
		{
			GD.PrintErr($"[PlayerInput] rawMovementVector out of range");
			movementInput.Update(delta, false, Vector2.Zero);
		}

		// Action Main and Action Top
		var actionMainInput = inputs[(int)InputPositionType.ActionMain];
		var actionTopInput = inputs[(int)InputPositionType.ActionTop];
		Vector2 rawActionVector = new Vector2(
			Input.GetJoyAxis(0, JoyAxis.RightX),
			Input.GetJoyAxis(0, JoyAxis.RightY)
		);

		// Get which action is pressed
		bool[] isActionPressed = new bool[] {
			Input.GetJoyAxis(0, JoyAxis.TriggerRight) > 0.5f,
			Input.GetJoyAxis(0, JoyAxis.TriggerLeft) > 0.5f
		};

		actionMainInput.Update(delta, isActionPressed[0], rawActionVector);
		actionTopInput.Update(delta, isActionPressed[1], rawActionVector);
	}

	private void UpdateKeyboardInputs(double delta)
	{
		// Movement
		var movementInput = inputs[(int)InputPositionType.Movement];
		Vector2 moveVector = new Vector2(
			Convert.ToInt32(Input.IsKeyPressed(Key.D)) - Convert.ToInt32(Input.IsKeyPressed(Key.A)),
			Convert.ToInt32(Input.IsKeyPressed(Key.S)) - Convert.ToInt32(Input.IsKeyPressed(Key.W))
		).Normalized();

		movementInput.Update(delta, true, moveVector);

		// Action Main and Action Top
		var actionMainInput = inputs[(int)InputPositionType.ActionMain];
		var actionTopInput = inputs[(int)InputPositionType.ActionTop];

		bool isActionMainPressed = Input.IsMouseButtonPressed(MouseButton.Left);
		bool isActionTopPressed = Input.IsMouseButtonPressed(MouseButton.Right);
		bool ignoreMouseDirection = Input.IsKeyPressed(Key.Ctrl);

		// Update with error handling
		try
		{
			actionMainInput.Update(delta, isActionMainPressed, ignoreMouseDirection ? Vector2.Zero : GetMouseDirection());
			actionTopInput.Update(delta, isActionTopPressed, ignoreMouseDirection ? Vector2.Zero : GetMouseDirection());
		}
		catch (ArgumentOutOfRangeException e)
		{
			GD.PrintErr($"[PlayerInput] Mouse direction out of range");
		}
	}

	private Vector2 GetMouseDirection()
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 screenCenter = new Vector2(GetViewport().GetVisibleRect().Size.X / 2, GetViewport().GetVisibleRect().Size.Y / 2);
		return (mousePos - screenCenter).Normalized();
	}

	public InputValues GetInput(InputPositionType type)
	{
		return inputs[(int)type];
	}
}
