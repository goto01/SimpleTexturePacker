using System.Collections.Generic;
using System.Linq;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Windows
{
	public class TextureRepositoryBrowserWindow : EditorWindow
	{
		private const int MaxScalingValue = 10; 
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
		
		private float HalfWindowWidth {get { return (position.width - Margin) / 2; }}
		
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
			GUILayout.Space(EditorGUIUtility.singleLineHeight*2);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical(GUILayout.Width(HalfWindowWidth));
			DrawTextureRepositoryTree();
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginVertical(GUILayout.Width(HalfWindowWidth));
			DrawSelectionInspector();
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.EndHorizontal();
		}

		private void Init()
		{
			if (_textureRepositories == null) LoadTextureRepositories();
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
			var value = EditorGUILayout.IntField(ScalingSpritePreviewValue, EditorPrefs.GetInt(ScalingSpritePreviewValue), GUILayout.Width(250));
			value = Mathf.Clamp(value, 1, MaxScalingValue);
			EditorPrefs.SetInt(ScalingSpritePreviewValue, value);
			EditorGUILayout.EndHorizontal();
		}

		private void DrawTextureRepositorySelectorEditor()
		{
			var index = _textureRepositories.IndexOf(_textureRepository);
			index = EditorGUILayout.Popup("Texture repository", index, _textureRepositoryNames);
			_textureRepository = _textureRepositories[index];
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
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(depth * IndentWidth);
				GUILayout.Label(SimpleItemPrefix, GUILayout.Width(IndentWidth));
				if (GUILayout.Button(f.Name, EditorStyles.miniButtonLeft)) f.Collapsed = !f.Collapsed;
				if (GUILayout.Button("Select", EditorStyles.miniButtonRight, GUILayout.Width(50))) SelectSprites(f.SpriteDescriptions);
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
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(spriteDescription.FileName, GUI.skin.label))
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
		}

		private void DrawSelectionInspector()
		{
			_selectionScroll = EditorGUILayout.BeginScrollView(_selectionScroll, GUI.skin.box);
			foreach (var selectedSprite in _selectedSprites)
			{
				EditorGUILayout.LabelField(selectedSprite.FileName);
				var width = GUILayoutUtility.GetLastRect().width;
				var height = width * selectedSprite.Sprite.rect.height / selectedSprite.Sprite.rect.width;
				if (height > 1) selectedSprite.Height = height;
				var rect = GUILayoutUtility.GetRect(0, selectedSprite.Height/EditorPrefs.GetInt(ScalingSpritePreviewValue));
				rect.width /= EditorPrefs.GetInt(ScalingSpritePreviewValue);
				var spriteRect = new Rect(selectedSprite.Sprite.rect.x/selectedSprite.Sprite.texture.width,
					selectedSprite.Sprite.rect.y/selectedSprite.Sprite.texture.height,
					selectedSprite.Sprite.rect.width/selectedSprite.Sprite.texture.width,
					selectedSprite.Sprite.rect.height/selectedSprite.Sprite.texture.height);
					
				GUI.Box(rect, string.Empty);
				GUI.DrawTextureWithTexCoords(rect, selectedSprite.Sprite.texture, spriteRect);
			}
			EditorGUILayout.EndScrollView();
		}

		private void SelectSprite(SpriteDescription spriteDescription)
		{
			if (_selectedSprites.Contains(spriteDescription))
			{
				_selectedSprites.Remove(spriteDescription);
				return;
			}
			_selectedSprites.Add(spriteDescription);
			Repaint();
		}

		private void SelectSpriteOnly(SpriteDescription spriteDescription)
		{
			_selectedSprites.Clear();
			_selectedSprites.Add(spriteDescription);
			Repaint();
		}

		private void SelectSprites(List<SpriteDescription> spriteDescriptions)
		{
			foreach (var spriteDescription in spriteDescriptions) SelectSprite(spriteDescription);
		}

		private bool CheckForSelection(SpriteDescription spriteDescription)
		{
			return _selectedSprites.Contains(spriteDescription);
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
	}
}