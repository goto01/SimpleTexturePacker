using TexturePacker.Editor.Publishing;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Windows
{
	public class PublishSettingsPreferences : EditorWindow
	{
		[PreferenceItem("Texture packer")]
		public static void DrawPublishSettingsPreferences()
		{
			EditorGUILayout.LabelField("Publish settings", EditorStyles.boldLabel);
			EditorPrefs.SetString(TexturePackerPublishing.TexturePackerPath, EditorGUILayout.TextField(TexturePackerPublishing.TexturePackerPath, 
				EditorPrefs.GetString(TexturePackerPublishing.TexturePackerPath)));
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TexturePackerPublishing.DestinationTexturePath, GUILayout.Width(150));
			EditorGUILayout.LabelField("Assets", GUILayout.Width(50));
			EditorGUILayout.TextField(EditorPrefs.GetString(TexturePackerPublishing.DestinationTexturePath));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TexturePackerPublishing.DestinationDataPath, GUILayout.Width(150));
			EditorGUILayout.LabelField("Assets", GUILayout.Width(50));
			EditorGUILayout.TextField(EditorPrefs.GetString(TexturePackerPublishing.DestinationDataPath));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.TextField(TexturePackerPublishing.SourceFolder, 
				EditorPrefs.GetString(TexturePackerPublishing.SourceFolder));
			EditorGUILayout.IntField(TexturePackerPublishing.ShapePadding, 
				EditorPrefs.GetInt(TexturePackerPublishing.ShapePadding));
			EditorGUILayout.IntField(TexturePackerPublishing.Extrude, 
				EditorPrefs.GetInt(TexturePackerPublishing.Extrude));
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.IntField(TexturePackerPublishing.FixedWidth,
				EditorPrefs.GetInt(TexturePackerPublishing.FixedWidth));
			EditorGUILayout.IntField(TexturePackerPublishing.FixedHeight,
				EditorPrefs.GetInt(TexturePackerPublishing.FixedHeight));
			EditorGUILayout.EndHorizontal();
		}
	}
}