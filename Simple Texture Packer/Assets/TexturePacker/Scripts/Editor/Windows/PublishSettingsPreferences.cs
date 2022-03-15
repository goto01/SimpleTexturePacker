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
		}
	}
}