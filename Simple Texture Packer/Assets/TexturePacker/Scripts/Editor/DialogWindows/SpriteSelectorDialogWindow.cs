using System.Collections.Generic;
using System.Linq;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.DialogWindows
{
	public class SpriteSelectorDialogWindow : BaseDialogWindow<SpriteSelectorDialogWindow>
	{
		private const int MaxScalingValue = 50; 
		private const int MaxDepth = 100;
		private const int IndentWidth = 10;
		private const float ScaleButtonWidth = 30;
		private const string SimpleItemPrefix = "├";
		private const string LastItemPrefix = "└";
		private readonly Color _defaultLabelColor = new Color(.7f, .7f, .7f);
		private const string ScalingSpritePreviewValue = "Scaling Sprite Preview Value Selector";

		private SpriteDescription _selectedSpriteDescription;
		private TextureRepository _textureRepository;
		private List<TextureRepository> _textureRepositories;
		private string[] _textureRepositoryNames;
		private Vector2 _textureRepositoryTreeScroll;
		private Vector2 _previewScroll;
		
		private float TreeEditorHeight {get { return _size.y / 2f; }}
		private float PreviewEditorHeight {get { return _size.y / 2f; }}
		private Rect ScaleMinRect{get{return new Rect(_size.x - 2 * ScaleButtonWidth, _size.y - ScaleButtonWidth, ScaleButtonWidth, ScaleButtonWidth);}}
		private Rect ScaleMaxRect{get{return new Rect(_size.x - ScaleButtonWidth, _size.y - ScaleButtonWidth, ScaleButtonWidth, ScaleButtonWidth);}}
		private int ScalingValue
		{
			get { return PlayerPrefs.GetInt(ScalingSpritePreviewValue); }
			set
			{
				value = Mathf.Clamp(value, 1, MaxScalingValue);
				PlayerPrefs.SetInt(ScalingSpritePreviewValue, value);
			}
		}
		public Sprite SelectedSprite { get { return _selectedSpriteDescription.Sprite; } }
		
		protected override void DrawContentEditor()
		{
			_resizable = true;
			_size = new Vector2(400, 380);
			Init();
			DrawRepositorySelector();
			DrawTextureRepositoryTree();
			if (_selectedSpriteDescription == null || _selectedSpriteDescription.Sprite == null) return;
			DrawEditorPreview();
			DrawScaleEditor();
		}

		private void Init()
		{
			if (_textureRepositories == null) LoadTextureRepositories();
		}
		
		private void LoadTextureRepositories()
		{
			_textureRepositories = AssetDatabaseHelper.LoadAllAssets<TextureRepository>();
			_textureRepositoryNames = _textureRepositories.Select(x => x.name).ToArray();
			if (_textureRepositories.Count > 0) _textureRepository = _textureRepositories[0];
		}

		private void DrawRepositorySelector()
		{
			var index = _textureRepositories.IndexOf(_textureRepository);
			index = EditorGUILayout.Popup("Texture repository", index, _textureRepositoryNames);
			_textureRepository = _textureRepositories[index];
		}
		
		private void DrawTextureRepositoryTree()
		{
			_textureRepositoryTreeScroll = EditorGUILayout.BeginScrollView(_textureRepositoryTreeScroll, GUI.skin.box, GUILayout.Height(TreeEditorHeight));
			DrawFolder(_textureRepository.Root, 0);
			EditorGUI.indentLevel = 0;
			EditorGUILayout.EndScrollView();
		}

		private void DrawFolder(Folder folder, int depth)
		{
			if (depth > MaxDepth)
			{
				Debug.LogError("Limit of depth");
				return;
			}
			foreach (var f in folder.Folders)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(depth * IndentWidth);
				GUILayout.Label(SimpleItemPrefix, GUILayout.Width(IndentWidth));
				if (GUILayout.Button(f.Name, EditorStyles.miniButton)) f.Collapsed = !f.Collapsed;
				EditorGUILayout.EndHorizontal();
				if (!f.Collapsed) DrawFolder(f, depth+1);
			}
			for (var index = 0; index < folder.SpriteDescriptions.Count; index++)
			{
				var spriteDescription = folder.SpriteDescriptions[index];
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(depth * IndentWidth);
				GUILayout.Label(index == folder.SpriteDescriptions.Count-1 ? LastItemPrefix : SimpleItemPrefix, GUILayout.Width(IndentWidth));
				if (!CheckForSelection(spriteDescription)) GUI.color = _defaultLabelColor;
				if (GUILayout.Button(spriteDescription.FileName, GUI.skin.label))
				{
					if (_selectedSpriteDescription == spriteDescription) RaiseYes();
					_selectedSpriteDescription = spriteDescription;
				}
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
			}
		}

		private void DrawEditorPreview()
		{
			EditorGUILayout.BeginVertical(GUILayout.Height(PreviewEditorHeight));
			_previewScroll = EditorGUILayout.BeginScrollView(_previewScroll);
			var rect = GUILayoutUtility.GetRect(0, _selectedSpriteDescription.Sprite.rect.height * ScalingValue);
			rect.width = _selectedSpriteDescription.Sprite.rect.width * ScalingValue;
			rect.height = _selectedSpriteDescription.Sprite.rect.height * ScalingValue;
			var spriteRect = new Rect(_selectedSpriteDescription.Sprite.rect.x/_selectedSpriteDescription.Sprite.texture.width,
				_selectedSpriteDescription.Sprite.rect.y/_selectedSpriteDescription.Sprite.texture.height,
				_selectedSpriteDescription.Sprite.rect.width/_selectedSpriteDescription.Sprite.texture.width,
				_selectedSpriteDescription.Sprite.rect.height/_selectedSpriteDescription.Sprite.texture.height);
					
			GUI.Box(rect, string.Empty);
			GUI.DrawTextureWithTexCoords(rect, _selectedSpriteDescription.Sprite.texture, spriteRect);	
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}

		private void DrawScaleEditor()
		{
			if (GUI.Button(ScaleMinRect, "-")) ScalingValue--;
			if (GUI.Button(ScaleMaxRect, "+")) ScalingValue++;
		}

		private bool CheckForSelection(SpriteDescription spriteDescription)
		{
			return spriteDescription == _selectedSpriteDescription;
		}
	}
}