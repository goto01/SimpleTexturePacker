using System;
using TexturePacker.Editor.DialogWindows;
using TexturePacker.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace TexturePacker.Editor.Properties
{
	[CustomPropertyDrawer(typeof(SpritePropertyAttribute))]
	public class SpriteProperty : PropertyDrawer
	{
		private const string Preview = "Sprite Property Preiew Enalbing";
		private const float SpritePreviewHeight = 100;
		private const float Separator = .8f;
		private Sprite _selectedSprite;
		private SerializedProperty _serializedProperty;
		private bool _heightMode;

		private bool PreviewFoldout
		{
			get { return PlayerPrefs.GetInt(Preview, -1) > 0; }
			set { PlayerPrefs.SetInt(Preview, value ? 1 : -1); }
		}
		private Sprite Sprite
		{
			get
			{
				if (_serializedProperty == null) return null;
				return (Sprite) _serializedProperty.objectReferenceValue;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var height = _heightMode ? base.GetPropertyHeight(property, label) : EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 3;
			if (PreviewFoldout) height += SpritePreviewHeight + 2 * EditorGUIUtility.standardVerticalSpacing;
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			_serializedProperty = property;
			if (_selectedSprite != null)
			{
				property.objectReferenceValue = _selectedSprite;
				_selectedSprite = null;
			}
			position.height = EditorGUIUtility.singleLineHeight;
			var tempWidth = position.width;
			var tempX = position.x;
			var rect = position;
			rect.width *= Separator;
			EditorGUI.PropertyField(rect, property, label);
			rect.x += rect.width;
			rect.width = position.width * (1 - Separator);
			if (GUI.Button(rect, "Select")) Dialog.ShowDialog<SpriteSelectorDialogWindow>("Sprite selector dialog", DialogType.YesNo).Yes += OnYes;
			rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			rect.width = tempWidth;
			rect.x = tempX;
			if ((_heightMode = (Sprite == null))) return;
			PreviewFoldout = EditorGUI.Foldout(rect, PreviewFoldout, Sprite.name);
			if (!PreviewFoldout) return;
			rect.height = SpritePreviewHeight;
			var spriteRect = new Rect(Sprite.rect.x/Sprite.texture.width,
				Sprite.rect.y/Sprite.texture.height,
				Sprite.rect.width/Sprite.texture.width,
				Sprite.rect.height/Sprite.texture.height);
			rect.width = rect.height * Sprite.rect.width / Sprite.rect.height;
			rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			var texture = new Texture2D((int)Sprite.rect.width, (int)Sprite.rect.height, TextureFormat.ARGB32, false);
			texture.SetPixels(Sprite.texture.GetPixels((int)Sprite.textureRect.x, (int)Sprite.textureRect.y,
				(int)Sprite.textureRect.width, (int)Sprite.textureRect.height));
			texture.filterMode = FilterMode.Point;
			texture.Apply();
			rect.height = SpritePreviewHeight;
			rect.width = rect.height * texture.width / texture.height;
			var style = new GUIStyle();
			style.normal.background = texture;
			GUI.Box(rect, GUIContent.none);
			GUI.Label(rect, GUIContent.none, style);
		}

		private void OnYes(SpriteSelectorDialogWindow sender)
		{
			_selectedSprite = sender.SelectedSprite;
			
		}
	}
}