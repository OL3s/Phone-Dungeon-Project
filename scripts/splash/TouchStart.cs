using Godot;
using SceneUtil;

public partial class TouchStart : Control
{
	private bool _isPressed;
	private bool _didHold;
	private double _holdTime;
	private const double HoldThreshold = 0.3;

	public override void _GuiInput(InputEvent e)
	{
		if (e is InputEventMouseButton mb && mb.ButtonIndex == MouseButton.Left)
		{
			if (mb.Pressed)
			{
				_isPressed = true;
				_didHold = false;
				_holdTime = 0;
				AcceptEvent();
			}
			else // release
			{
				if (_isPressed && !_didHold)
					DoA(); // short click
				_isPressed = false;
				AcceptEvent();
			}
		}
	}

	public override void _Process(double delta)
	{
		if (!_isPressed) return;

		_holdTime += delta;
		if (!_didHold && _holdTime >= HoldThreshold)
		{
			_didHold = true;
			DoB(); // hold triggered once
		}
	}

	private void DoA() => SceneUtil.SceneChanger.GoTo("res://scenes/rom-menu.tscn");
	private void DoB() => GD.Print("Held → Do B");
}
