using Editor.Windows.DialogWindows;
using UnityEditor;
using UnityEngine;

namespace Editor.TexturePacker.DialogWindows
{
	public class CreateTextureDescriptionDialogWindow : BaseDialogWindow<CreateTextureDescriptionDialogWindow>
	{
		public string Name;
		public TextAsset JsonDataFile;
		public Texture2D Texture2D;
		
		protected override void DrawContentEditor()
		{
			_size = new Vector2(400, 150);
			Name = EditorGUILayout.TextField("Name", Name);
			JsonDataFile = EditorGUILayout.ObjectField("Json Data File", JsonDataFile, typeof(TextAsset), false) as TextAsset;
			Texture2D = EditorGUILayout.ObjectField("Texture", Texture2D, typeof(Texture2D), false) as Texture2D;
			_yesPossible = !string.IsNullOrEmpty(Name) && JsonDataFile != null && Texture2D != null;
		}
	}
}