using System;
using System.Collections.Generic;
using System.Linq;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.DialogWindows
{
	public class SpriteSelectorDialogWindow : BaseDialogWindow<SpriteSelectorDialogWindow>
	{
		private float Height = 480;
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
		private string _searchFilter;
		private string _actualSearchFilter;
		private string _prevSearchFilter;
		
		private float TreeEditorHeight {get { return 200; }}
		private float PreviewEditorHeight {get { return 200; }}
		private Rect ScaleMinRect{get{return new Rect(_size.x - 3 * ScaleButtonWidth, 440 - ScaleButtonWidth, ScaleButtonWidth, ScaleButtonWidth);}}
		private Rect ScaleMaxRect{get{return new Rect(_size.x - 2 * ScaleButtonWidth, 440 - ScaleButtonWidth, ScaleButtonWidth, ScaleButtonWidth);}}
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
		private bool Searching{get { return !string.IsNullOrEmpty(_searchFilter); }}
		public string SearchingFilter
		{
			get { return _actualSearchFilter; }
			set
			{
				_searchFilter = _prevSearchFilter = value;
				_actualSearchFilter = value.ToUpper();
			}
		}
		
		protected override void DrawContentEditor()
		{
			_size = new Vector2(_parentRect.width, Height);
			Init();
			DrawRepositorySelector();
			DrawSearchEditor();
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

		private void DrawSearchEditor()
		{
			EditorGUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
			_searchFilter = EditorGUILayout.TextField(_searchFilter, GUI.skin.FindStyle("ToolbarSeachTextField"));
			if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton"))) _searchFilter = string.Empty;
			EditorGUILayout.EndHorizontal();

			if (!string.Equals(_prevSearchFilter, _searchFilter, StringComparison.InvariantCultureIgnoreCase))
				_actualSearchFilter = _searchFilter.ToUpper();
			_prevSearchFilter = _searchFilter;
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
				DrawFolderEditor(f, depth);
			for (var index = 0; index < folder.SpriteDescriptions.Count; index++)
			{
				var spriteDescription = folder.SpriteDescriptions[index];
				DrawSpriteEditor(spriteDescription, folder, index, depth);
			}
		}

		private void DrawFolderEditor(Folder folder, int depth)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(depth * IndentWidth);
			GUILayout.Label(SimpleItemPrefix, GUILayout.Width(IndentWidth));
			if (GUILayout.Button(folder.Name, EditorStyles.miniButton)) folder.Collapsed = !folder.Collapsed;
			EditorGUILayout.EndHorizontal();
			if (Searching || !folder.Collapsed) DrawFolder(folder, depth+1);
		}

		private void DrawSpriteEditor(SpriteDescription spriteDescription, Folder folder, int index, int depth)
		{
			if (Searching && !spriteDescription.FileName.ToUpper().Contains(_actualSearchFilter)) return;
			
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

		private void DrawEditorPreview()
		{
			EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(PreviewEditorHeight));
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