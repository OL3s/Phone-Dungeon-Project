using Godot;
using System;
using System.Text.Json;

namespace Json
{
	public static class JsonManager
	{
		private static readonly JsonSerializerOptions Options = new()
		{
			WriteIndented = true,
			PropertyNameCaseInsensitive = true,
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true,
			IncludeFields = true
		};

		// Load typed object (returns fallback if missing/invalid)
		public static T? Load<T>(string path, T? fallback = default)
		{
			try
			{
				if (!FileAccess.FileExists(path)) return fallback;
				using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				var json = f.GetAsText();
				return JsonSerializer.Deserialize<T>(json, Options) ?? fallback;
			}
			catch (Exception e)
			{
				GD.PushError($"JsonManager.Load<{typeof(T).Name}>({path}): {e.Message}");
				return fallback;
			}
		}

		// Save typed object
		public static bool Save<T>(string path, T data)
		{
			try
			{
				var dir = System.IO.Path.GetDirectoryName(path);
				if (!string.IsNullOrEmpty(dir))
				{
					var abs = ProjectSettings.GlobalizePath(dir);
					DirAccess.MakeDirRecursiveAbsolute(abs);
				}

				using var f = FileAccess.Open(path, FileAccess.ModeFlags.Write);
				var json = JsonSerializer.Serialize(data, Options);
				f.StoreString(json);
				return true;
			}
			catch (Exception e)
			{
				GD.PushError($"JsonManager.Save<{typeof(T).Name}>({path}): {e.Message}");
				return false;
			}
		}

		// (Optional) Godot Dictionary versions
		public static Godot.Collections.Dictionary LoadDict(string path)
		{
			try
			{
				if (!FileAccess.FileExists(path)) return new();
				using var f = FileAccess.Open(path, FileAccess.ModeFlags.Read);
				var json = f.GetAsText();
				var v = Godot.Json.ParseString(json);
				return v.VariantType == Variant.Type.Dictionary ? (Godot.Collections.Dictionary)v : new();
			}
			catch (Exception e)
			{
				GD.PushError($"JsonManager.LoadDict({path}): {e.Message}");
				return new();
			}
		}

		public static bool SaveDict(string path, Godot.Collections.Dictionary data)
		{
			try
			{
				var dir = System.IO.Path.GetDirectoryName(path);
				if (!string.IsNullOrEmpty(dir))
				{
					var abs = ProjectSettings.GlobalizePath(dir);
					DirAccess.MakeDirRecursiveAbsolute(abs);
				}

				using var f = FileAccess.Open(path, FileAccess.ModeFlags.Write);
				var json = Godot.Json.Stringify(data);
				f.StoreString(json);
				return true;
			}
			catch (Exception e)
			{
				GD.PushError($"JsonManager.SaveDict({path}): {e.Message}");
				return false;
			}
		}
	}
}
