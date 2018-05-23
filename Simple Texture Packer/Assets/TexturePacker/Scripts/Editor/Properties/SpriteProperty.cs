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
		private const float EditorOffset = 16f + 2f;
		private const string Preview = "Sprite Property Preiew Enalbing";
		private const float SpritePreviewHeight = 100;
		private const float Separator = .8f;
		private Sprite _prevSprite;
		private Sprite _selectedSprite;
		private Sprite _sprite;
		private bool _previewPossible;
		private Texture2D _previewTexture;
		private GUIStyle _previewTextureStyle;

		private bool NeedRefreshPreviewTexture{get { return _prevSprite != _sprite; }}
		private bool PreviewNotPossible { get { return _previewPossible = (_sprite == null); } }
		private bool PreviewFoldout
		{
			get { return PlayerPrefs.GetInt(Preview, -1) > 0; }
			set { PlayerPrefs.SetInt(Preview, value ? 1 : -1); }
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			if (!PreviewNotPossible) height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			if (!PreviewNotPossible && PreviewFoldout) height += EditorGUIUtility.standardVerticalSpacing + SpritePreviewHeight;
			height += EditorGUIUtility.standardVerticalSpacing;
			return height;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init();
			_sprite = property.objectReferenceValue as Sprite;
			ApplySelectedSprite(property);
			position.height = EditorGUIUtility.singleLineHeight;
			DrawSpriteSelector(position, property, label);
			position.y += EditorOffset;
			if (PreviewNotPossible) return;
			DrawFoldout(position);
			if (!PreviewFoldout) return;
			position.y += EditorOffset;
			DrawSpritePreview(position);
		}

		private void Init()
		{
			if (_previewTextureStyle == null) _previewTextureStyle = new GUIStyle(); 
		}

		private void ApplySelectedSprite(SerializedProperty property)
		{
			if (_selectedSprite == null) return;
			property.objectReferenceValue = _selectedSprite;
			_selectedSprite = null;
		}
		
		private void DrawSpriteSelector(Rect rect, SerializedProperty property, GUIContent label)
		{
			var width = rect.width;
			rect.width *= Separator;
			EditorGUI.PropertyField(rect, property, label);
			rect.x += rect.width;
			rect.width = width * (1 - Separator);
			if (GUI.Button(rect, "Select")) Dialog.ShowDialog<SpriteSelectorDialogWindow>("Sprite selector dialog", DialogType.YesNo).Yes += OnYes;
		}

		private void DrawFoldout(Rect rect)
		{
			PreviewFoldout = EditorGUI.Foldout(rect, PreviewFoldout, _sprite.name);
		}

		private void DrawSpritePreview(Rect rect)
		{
			if (NeedRefreshPreviewTexture) RefreshPreviewTexture();
			rect.height = SpritePreviewHeight;
			rect.width = rect.height * _previewTexture.width / _previewTexture.height;
			GUI.Box(rect, GUIContent.none);
			GUI.Label(rect, GUIContent.none, _previewTextureStyle);
		}

		private void RefreshPreviewTexture()
		{
			var texture = new Texture2D((int)_sprite.rect.width, (int)_sprite.rect.height, TextureFormat.ARGB32, false);
			texture.SetPixels(_sprite.texture.GetPixels((int)_sprite.textureRect.x, (int)_sprite.textureRect.y,
				(int)_sprite.textureRect.width, (int)_sprite.textureRect.height));
			texture.filterMode = FilterMode.Point;
			texture.Apply();
			_prevSprite = _sprite;
			_previewTexture = texture;
			_previewTextureStyle.normal.background = _previewTexture;
		}

		private void OnYes(SpriteSelectorDialogWindow sender)
		{
			_selectedSprite = sender.SelectedSprite;
			
		}
	}
}