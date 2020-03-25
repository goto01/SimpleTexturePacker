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
			EditorPrefs.SetString(TexturePackerPublishing.DestinationTexturePath, EditorGUILayout.TextField(EditorPrefs.GetString(TexturePackerPublishing.DestinationTexturePath)));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TexturePackerPublishing.DestinationTexturePath, GUILayout.Width(150));
			EditorGUILayout.LabelField("Assets", GUILayout.Width(50));
			EditorPrefs.SetString(TexturePackerPublishing.DestinationDataPath, EditorGUILayout.TextField(EditorPrefs.GetString(TexturePackerPublishing.DestinationDataPath)));
			EditorGUILayout.EndHorizontal();
			EditorPrefs.SetString(TexturePackerPublishing.SourceFolder, EditorGUILayout.TextField(TexturePackerPublishing.SourceFolder, 
				EditorPrefs.GetString(TexturePackerPublishing.SourceFolder)));
			EditorPrefs.SetInt(TexturePackerPublishing.ShapePadding, EditorGUILayout.IntField(TexturePackerPublishing.ShapePadding, 
				EditorPrefs.GetInt(TexturePackerPublishing.ShapePadding)));
			EditorPrefs.SetInt(TexturePackerPublishing.Extrude, EditorGUILayout.IntField(TexturePackerPublishing.Extrude, 
				EditorPrefs.GetInt(TexturePackerPublishing.Extrude)));
			
			if (GUILayout.Button("Publish")) TexturePackerPublishing.Publish();
		}
	}
}