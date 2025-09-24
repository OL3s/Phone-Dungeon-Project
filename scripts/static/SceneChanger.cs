using Godot;

namespace SceneUtil
{
	public static class SceneChanger
	{
		public static void GoTo(string path)
		{
			var scene = ResourceLoader.Load<PackedScene>(path);
			if (scene != null)
			{
				var tree = (SceneTree)Engine.GetMainLoop();
				tree.ChangeSceneToPacked(scene);
			}
		}
	}
}
