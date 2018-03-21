using TexturePacker.Editor.Publishing;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Windows
{
	public class PublishSettingsWindow : EditorWindow
	{
		public static void ShowSelf()
		{
			EditorWindow.GetWindow<PublishSettingsWindow>(false, "Publish Settings Window");
		}
		
		private void OnGUI()
		{
			EditorGUILayout.LabelField("Publish settings", EditorStyles.boldLabel);
			EditorPrefs.SetString(TexturePackerPublishing.TexturePackerPath, EditorGUILayout.TextField(TexturePackerPublishing.TexturePackerPath, 
				EditorPrefs.GetString(TexturePackerPublishing.TexturePackerPath)));
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TexturePackerPublishing.DestinationTexturePath, GUILayout.Width(150));
			EditorGUILayout.LabelField(Application.dataPath, GUILayout.Width(370));
			EditorPrefs.SetString(TexturePackerPublishing.DestinationTexturePath, EditorGUILayout.TextField(EditorPrefs.GetString(TexturePackerPublishing.DestinationTexturePath)));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TexturePackerPublishing.DestinationTexturePath, GUILayout.Width(150));
			EditorGUILayout.LabelField(Application.dataPath, GUILayout.Width(370));
			EditorPrefs.SetString(TexturePackerPublishing.DestinationDataPath, EditorGUILayout.TextField(EditorPrefs.GetString(TexturePackerPublishing.DestinationDataPath)));
			EditorGUILayout.EndHorizontal();
			EditorPrefs.SetString(TexturePackerPublishing.SourceFolder, EditorGUILayout.TextField(TexturePackerPublishing.SourceFolder, 
				EditorPrefs.GetString(TexturePackerPublishing.SourceFolder)));
			
			if (GUILayout.Button("Publish")) TexturePackerPublishing.Publish();
		}
	}
}