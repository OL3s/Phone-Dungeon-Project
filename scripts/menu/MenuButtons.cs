using Godot;

public partial class MenuButtons : HBoxContainer
{
	// Menu button pressed signal
	[Signal] public delegate void MenuChangeButtonPressedEventHandler(int index);

	public override void _Ready()
	{
		
		// Subscribe to button press
		for (int i = 0; i < GetChildCount(); i++)
		{
			if (GetChild(i) is BaseButton btn)
			{
				int idx = i; // capture
				btn.Pressed += () => OnPressed(idx);
			}
		}
		
		// Start at page 2
		OnPressed(2); // Home
	}

	private void OnPressed(int idx)
	{
		// Emit signal pressed;
		EmitSignal(SignalName.MenuChangeButtonPressed, idx);
		
		// Dim all buttons
		foreach (var child in GetChildren())
			if (child is TextureButton b)
				b.SelfModulate = new Color(1, 1, 1, 0.5f);

		// Highlight selected
		if (GetChild(idx) is TextureButton sel)
			sel.SelfModulate = new Color(1, 1, 1, 1f);
	}
}
