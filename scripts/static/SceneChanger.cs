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
                // Defer the change to avoid processing while nodes are being removed
                tree.CallDeferred("change_scene_to_packed", scene);
            }
        }
    }
}