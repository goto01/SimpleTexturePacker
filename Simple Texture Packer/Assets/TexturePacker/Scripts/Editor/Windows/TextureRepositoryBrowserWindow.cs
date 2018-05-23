using System;
using System.Collections.Generic;
using System.Linq;
using TexturePacker.Editor.Animation;
using TexturePacker.Editor.DialogWindows;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Windows
{
	public class TextureRepositoryBrowserWindow : EditorWindow
	{
		private const int MaxScalingValue = 50; 
		private const int Margin = 10;
		private const int MaxDepth = 100;
		private const int IndentWidth = 20;
		private const string SimpleItemPrefix = "├─";
		private const string LastItemPrefix = "└─";
		private readonly Color _defaultLabelColor = new Color(.7f, .7f, .7f);
		private const string ScalingSpritePreviewValue = "Scaling Sprite Preview Value";
		
		private TextureRepository _textureRepository;
		private List<TextureRepository> _textureRepositories;
		private string[] _textureRepositoryNames;
		private Vector2 _textureRepositoryTreeScroll;
		private List<SpriteDescription> _selectedSprites;
		private Vector2 _selectionScroll;
		private bool _ctrlPressed;
		private Folder _selectedFolder;
		private string _searchFilter;
		private string _actualSearchFilter;
		private string _prevSearchFilter;

		private int ScalingValue
		{
			get { return PlayerPrefs.GetInt(ScalingSpritePreviewValue); }
			set
			{
				value = Mathf.Clamp(value, 1, MaxScalingValue);
				PlayerPrefs.SetInt(ScalingSpritePreviewValue, value);
			}
		}
		private float HalfWindowWidth {get { return (position.width - Margin) / 2; }}
		private float WindowWidth{get { return position.width; }}
		private float WindowHeight{get { return position.height; }}
		private bool Searching{get { return !string.IsNullOrEmpty(_searchFilter); }}
		
		public static void ShowSelf()
		{
			EditorWindow.GetWindow<TextureRepositoryBrowserWindow>(false, "Texture Repository Browser Window");
		}
		
		protected virtual void OnGUI()
		{
			HandleKeys();
			DrawHeaderInspector();
			Init();
			if (_textureRepositories.Count == 0)
			{
				EditorGUILayout.HelpBox("You don't have any instance of TextureRepository", MessageType.Error);
				return;
			}
			DrawTextureRepositorySelectorEditor();
			GUILayout.Space(EditorGUIUtility.singleLineHeight);
			DrawSearchField();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.Width(HalfWindowWidth));
			DrawTextureRepositoryTree();
			EditorGUILayout.EndVertical();			
			EditorGUILayout.BeginVertical(GUILayout.Width(HalfWindowWidth));
			DrawSelectionInspector();
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();
			DrawScalingEditor();
			
			Repaint();
			EditorUtility.SetDirty(_textureRepository);
		}

		private void Init()
		{
			if (_textureRepositories == null || _textureRepositories.Count == 0) LoadTextureRepositories();
			if (_selectedSprites == null) _selectedSprites = new List<SpriteDescription>();
		}

		private void LoadTextureRepositories()
		{
			_textureRepositories = AssetDatabaseHelper.LoadAllAssets<TextureRepository>();
			_textureRepositoryNames = _textureRepositories.Select(x => x.name).ToArray();
			if (_textureRepositories.Count > 0) _textureRepository = _textureRepositories[0];
		}

		private void DrawHeaderInspector()
		{
			EditorGUILayout.LabelField("Texture Repository Browser", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Refresh")) LoadTextureRepositories();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawTextureRepositorySelectorEditor()
		{
			var index = _textureRepositories.IndexOf(_textureRepository);
			index = EditorGUILayout.Popup("Texture repository", index, _textureRepositoryNames);
			_textureRepository = _textureRepositories[index];
		}

		private void DrawSearchField()
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
			_textureRepositoryTreeScroll = EditorGUILayout.BeginScrollView(_textureRepositoryTreeScroll, GUI.skin.box);
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
				DrawSpriteEditor(folder.SpriteDescriptions[index], folder, index, depth);
		}

		private void DrawFolderEditor(Folder folder, int depth)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(depth * IndentWidth);
			GUILayout.Label(SimpleItemPrefix, GUILayout.Width(IndentWidth));
			if (folder != _selectedFolder) GUI.color = _defaultLabelColor;
			if (GUILayout.Button(folder.Name, EditorStyles.miniButtonLeft, GUILayout.MinWidth(50))) folder.Collapsed = !folder.Collapsed;
			if (GUILayout.Button("Select", EditorStyles.miniButtonMid, GUILayout.Width(50))) SelectFolder(folder);
			if (GUILayout.Button("Animation", EditorStyles.miniButtonMid, GUILayout.Width(70))) CreateAnimation(folder);
			if (GUILayout.Button("Sprites", EditorStyles.miniButtonRight, GUILayout.Width(50))) SelectSprites(folder.SpriteDescriptions);
			EditorGUILayout.EndHorizontal();
			GUI.color = Color.white;
			if (Searching || !folder.Collapsed) DrawFolder(folder, depth+1);
		}
		
		private void DrawSpriteEditor(SpriteDescription spriteDescription, Folder folder, int index, int depth)
		{
			if (Searching && !spriteDescription.FileName.ToUpper().Contains(_actualSearchFilter)) return;
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(depth * IndentWidth);
			GUILayout.Label(index == folder.SpriteDescriptions.Count-1 ? LastItemPrefix : SimpleItemPrefix, GUILayout.Width(IndentWidth));
			if (!CheckForSelection(spriteDescription)) GUI.color = _defaultLabelColor;
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button(spriteDescription.FileName, GUI.skin.label, GUILayout.MinWidth(50)))
			{
				if (_ctrlPressed) SelectSprite(spriteDescription);
				else SelectSpriteOnly(spriteDescription);
			}
			GUI.color = Color.white;
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Set sprite", EditorStyles.miniButton, GUILayout.Width(70))) SetSprite(spriteDescription.Sprite);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.EndHorizontal();
		}

		private void DrawSelectionInspector()
		{
			if (_selectedFolder != null)
			{
				DrawFolderSelectionInspector();
				return;
			}
			_selectionScroll = EditorGUILayout.BeginScrollView(_selectionScroll);
			foreach (var selectedSprite in _selectedSprites)
			{
				EditorGUILayout.BeginVertical(GUI.skin.box);
				EditorGUILayout.LabelField(selectedSprite.FileName);
				selectedSprite.Pivot = EditorGUILayout.Vector2Field("Pivot", selectedSprite.Pivot);
				var rect = GUILayoutUtility.GetRect(0, selectedSprite.Sprite.rect.height * ScalingValue);
				rect.width = selectedSprite.Sprite.rect.width * ScalingValue;
				rect.height = selectedSprite.Sprite.rect.height * ScalingValue;
				var spriteRect = new Rect(selectedSprite.Sprite.rect.x/selectedSprite.Sprite.texture.width,
					selectedSprite.Sprite.rect.y/selectedSprite.Sprite.texture.height,
					selectedSprite.Sprite.rect.width/selectedSprite.Sprite.texture.width,
					selectedSprite.Sprite.rect.height/selectedSprite.Sprite.texture.height);
					
				GUI.Box(rect, string.Empty);
				GUI.DrawTextureWithTexCoords(rect, selectedSprite.Sprite.texture, spriteRect);
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndScrollView();
		}

		private void DrawFolderSelectionInspector()
		{
			EditorGUILayout.BeginVertical(GUI.skin.box);
			_selectedFolder.FrameRate = EditorGUILayout.IntField("Frame Rate", _selectedFolder.FrameRate);
			_selectedFolder.Loop = EditorGUILayout.Toggle("Loop", _selectedFolder.Loop);
			EditorGUILayout.EndVertical();
		}

		private void DrawScalingEditor()
		{
			const float buttonHeight = 40;
			const float buttonWidth = 40;
			const float indent = 20;
			if (GUI.Button(new Rect(WindowWidth - indent - buttonWidth * 2, WindowHeight - indent - buttonHeight, buttonWidth, buttonHeight), "-", EditorStyles.miniButtonLeft))
				ScalingValue--;
			if (GUI.Button(new Rect(WindowWidth - indent - buttonWidth, WindowHeight - indent - buttonHeight, buttonWidth, buttonHeight), "+", EditorStyles.miniButtonRight))
				ScalingValue++;
		}

		private void SelectSprite(SpriteDescription spriteDescription)
		{
			if (_selectedSprites.Contains(spriteDescription))
			{
				_selectedSprites.Remove(spriteDescription);
				return;
			}
			_selectedSprites.Add(spriteDescription);
			_selectedFolder = null;
		}

		private void SelectSpriteOnly(SpriteDescription spriteDescription)
		{
			_selectedSprites.Clear();
			_selectedSprites.Add(spriteDescription);
			_selectedFolder = null;
		}

		private void SelectSprites(List<SpriteDescription> spriteDescriptions)
		{
			foreach (var spriteDescription in spriteDescriptions) SelectSprite(spriteDescription);
		}

		private bool CheckForSelection(SpriteDescription spriteDescription)
		{
			return _selectedSprites.Contains(spriteDescription);
		}

		private void SelectFolder(Folder folder)
		{
			_selectedSprites.Clear();
			_selectedFolder = folder;
		}
		
		private void HandleKeys()
		{
			var e = Event.current;
			switch (e.type)
			{
				case EventType.KeyDown:
					if (Event.current.keyCode == KeyCode.LeftControl) _ctrlPressed = true;
					else _ctrlPressed = false;
					break;
				case EventType.KeyUp:
					if (Event.current.keyCode == KeyCode.LeftControl) _ctrlPressed = false;
					break;
			}
		}

		private void SetSprite(Sprite sprite)
		{
			if (Selection.activeObject == null) return;
			var spriteRenderer = Selection.activeGameObject.GetComponent<SpriteRenderer>();
			if (spriteRenderer == null) return;
			spriteRenderer.sprite = sprite;
		}

		private void CreateAnimation(Folder folder)
		{
			var window = Dialog.ShowDialog<CreateAnimationWindow>("Create animation", DialogType.YesNo);
			window.Name = folder.Name;
			window.FrameRate = folder.FrameRate;
			window.IsLooping = folder.Loop;
			window.Sprites = folder.SpriteDescriptions.Select(x => x.Sprite).ToList();
			window.Yes += WindowOnYes;
		}

		private void WindowOnYes(CreateAnimationWindow sender)
		{
			var animation = AnimationGenerator.GenerateAndSaveAnimationToCurrentDirectory(sender.Sprites, sender.FrameRate, sender.IsLooping, sender.Name);
		}
	}
}