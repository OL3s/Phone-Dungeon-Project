using Godot;
using System;
using MyClasses;

public partial class GameData : Node
{
	[Export] public bool IncludeGlobalData;
	[Export] public bool IncludeGameData;
	[Export] public bool IncludeInventoryData;
	
	public MyClasses.GameData fileGame = null;
	public MyClasses.PermData fileGlobal = null;
	public MyClasses.InventoryData fileInventory = null;

	public void override _Ready(){
		
	}
}
