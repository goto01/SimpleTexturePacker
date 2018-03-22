using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.DialogWindows
{
	public class CreateAnimationWindow : BaseDialogWindow<CreateAnimationWindow>
	{
		public string Name;
		public int FrameRate;
		public bool IsLooping;
		public List<Sprite> Sprites;
		
		protected override void DrawContentEditor()
		{
			_size = new Vector2(300, 100);
			Name = EditorGUILayout.TextField("Name", Name);
			FrameRate = EditorGUILayout.IntField("Frame rate", FrameRate);
			IsLooping = EditorGUILayout.Toggle("Is looping", IsLooping);
		}
	}
}