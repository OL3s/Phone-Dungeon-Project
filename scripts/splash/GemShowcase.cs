using Godot;
using System;
using FileData;

public partial class GemShowcase : EffectFloat
{
	public override void _Ready()
	{
		base._Ready();
		
		// Init values
		SaveData saveData = null;
		
		/*
			Requisits:
			- Room needs Node with name SaveDava with this script
			- The GemShowcase's parent must have the {index + 1} as the last char in the name
		*/
		
		// Fetch object's SaveData class
		try { saveData = GetTree().Root.GetNode("Main").GetNode<SaveData>("SaveData"); }
		catch(Exception e) { GD.Print($"warning: problem fetching SaveData object\n{e.Message}\n{e.StackTrace}"); return;}

		// Fetch Index
		string nodeName = GetParent().Name.ToString();
		char lastChar = nodeName[nodeName.Length - 1];
		
		// Fetch Index debugger
		if (! char.IsDigit(lastChar)) {
			GD.Print("warning: gem parent missing index name");
			return;
		}
		
		// Convert char => int
		int index = lastChar - '0' - 1;

		// Add visible trait
		Visible = saveData.permData.Gems[index] != 0;

	}
}
