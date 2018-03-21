using System.Collections.Generic;
using System.Linq;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Editors
{
	[CustomEditor(typeof(TexturePackerSpriteSelector))]
	public class TexturePackerSpriteSelectorEditor : UnityEditor.Editor
	{
		private const int MaxDepth = 100;
		private const int IndentWidth = 20;
		private const string SimpleItemPrefix = "├─";
		private const string LastItemPrefix = "└─";
		private readonly Color _defaultLabelColor = new Color(.7f, .7f, .7f);
		
		private TextureRepository _textureRepository;
		private List<TextureRepository> _textureRepositories;
		private string[] _textureRepositoryNames;
		private Vector2 _textureRepositoryTreeScroll;
		private SpriteRenderer _spriteRenderer;

		private Sprite Sprite
		{
			get { return _spriteRenderer.sprite; }
			set { _spriteRenderer.sprite = value; }
		}
		
		private void OnEnable()
		{
			LoadTextureRepositories();
		}

		public override void OnInspectorGUI()
		{
			FindProperties();
			Init();
			if (_textureRepositories.Count == 0)
			{
				EditorGUILayout.HelpBox("You don't have any instance of TextureRepository", MessageType.Error);
				return;
			}
			DrawTextureRepositorySelectorEditor();
			DrawTextureRepositoryTree();
		}

		private void FindProperties()
		{
			_spriteRenderer = serializedObject.FindProperty("_spriteRenderer").objectReferenceValue as SpriteRenderer;
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
				if (GUILayout.Button(spriteDescription.FileName, GUI.skin.label)) Select(spriteDescription);
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
			}
		}

		private bool CheckForSelection(SpriteDescription spriteDescription)
		{
			return Sprite == spriteDescription.Sprite;
		}

		private void Select(SpriteDescription spriteDescription)
		{
			if (Sprite == spriteDescription.Sprite)
			{
				Sprite = null;
				return;
			}
			Sprite = spriteDescription.Sprite;
		}
	}
}