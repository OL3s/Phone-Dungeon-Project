using Godot;
using System;

public partial class MenuButtons : HBoxContainer
{
	[Signal]
	public delegate void MenuChangeButtonPressedEventHandler(int index);

	public override void _Ready()
	{
		int i = 0;
		foreach (Node child in GetChildren())
		{
			if (child is Button btn)
			{
				btn.SetMeta("index", i);
				btn.Pressed += () =>
				{
					int index = (int)btn.GetMeta("index");
					EmitSignal(SignalName.MenuChangeButtonPressed, index);
				};
				i++;
			}
		}
	}
}
