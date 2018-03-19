using Editor.TexturePacker.Domain;
using UnityEditor;
using UnityEngine;

namespace Editor.TexturePacker.Editors
{
	[CustomEditor(typeof(TextureDescription))]
	public class TextureDescriptionEditor : UnityEditor.Editor
	{
		private SerializedProperty _name;
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
			_name = serializedObject.FindProperty("Name");
			_textureRepository = serializedObject.FindProperty("TextureRepository");
		}
		
		private void DrawInitButtons()
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			if (_textureRepository.objectReferenceValue == null || string.IsNullOrEmpty(_name.stringValue)) GUI.enabled = false;
			if (GUILayout.Button("Convert"))
			{
				using (var transformation = new Transformation.Transformation(Target)) transformation.Transform();
			}
			GUI.enabled = true;
		}
	}
}