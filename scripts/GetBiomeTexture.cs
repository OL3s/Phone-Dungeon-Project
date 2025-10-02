using Godot;
using MyClasses;        // For Converters

public partial class GetBiomeTexture : TextureRect
{
	// Optional: if you prefer wiring via inspector, use NodePath export instead.
	// [Export] public NodePath SaveDataPath;

	public override void _Ready()
	{
		// If using NodePath:
		// var saveNode = GetNode<SaveData>(SaveDataPath);

		// Auto-locate like other scripts do (see scenes/LabelSaveData.cs):
		var saveNode = GetTree().Root.GetNode("Main").GetNode<SaveData>("SaveData");
		if (saveNode?.gameData != null)
		{
			Texture = Converters.BiomeToTexture(saveNode.gameData.Biome);
		}
	}
}
