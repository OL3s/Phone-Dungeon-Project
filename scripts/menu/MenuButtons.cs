using Godot;

public partial class MenuButtons : HBoxContainer
{
	[Signal] public delegate void MenuChangeButtonPressedEventHandler(int index);

	private int _selected = 0;

	public override void _Ready()
	{
		for (int i = 0; i < GetChildCount(); i++)
		{
			if (GetChild(i) is Button btn)
			{
				int idx = i; // capture
				btn.Pressed += () => OnPressed(idx);
			}
		}
	}

	private void OnPressed(int idx)
	{
		_selected = idx;
		EmitSignal(SignalName.MenuChangeButtonPressed, idx);	
	}
}
