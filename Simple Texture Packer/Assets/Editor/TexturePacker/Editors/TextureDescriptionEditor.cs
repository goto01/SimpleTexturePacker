using Editor.TexturePacker.Domain;
using UnityEditor;
using UnityEngine;

namespace Editor.TexturePacker.Editors
{
	[CustomEditor(typeof(TextureDescription))]
	public class TextureDescriptionEditor : UnityEditor.Editor
	{
		private SerializedProperty _textureRepository;
		
		private TextureDescription Target{get{return target as TextureDescription;}}
		
		public override void OnInspectorGUI()
		{
			FindProperties();
			DrawDefaultInspector();
			DrawInitButtons();
			serializedObject.ApplyModifiedProperties();
		}

		private void FindProperties()
		{
			_textureRepository = serializedObject.FindProperty("TextureRepository");
		}
		
		private void DrawInitButtons()
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (_textureRepository.objectReferenceValue == null) GUI.enabled = false;
			if (GUILayout.Button("Convert"))
			{
				using (var transformation = new Transformation.Transformation(Target)) transformation.Transform();
			}
			GUI.enabled = true;
		}
	}
}