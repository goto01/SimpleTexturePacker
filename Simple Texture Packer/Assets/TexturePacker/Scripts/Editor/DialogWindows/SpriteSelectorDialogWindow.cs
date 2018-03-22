using System.Collections.Generic;
using System.Linq;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.DialogWindows
{
	public class SpriteSelectorDialogWindow : BaseDialogWindow<SpriteSelectorDialogWindow>
	{
		private const int MaxDepth = 100;
		private const int IndentWidth = 20;
		private const string SimpleItemPrefix = "├─";
		private const string LastItemPrefix = "└─";
		private readonly Color _defaultLabelColor = new Color(.7f, .7f, .7f);

		private SpriteDescription _selectedSpriteDescription;
		private TextureRepository _textureRepository;
		private List<TextureRepository> _textureRepositories;
		private string[] _textureRepositoryNames;
		private Vector2 _textureRepositoryTreeScroll;
		
		protected override void DrawContentEditor()
		{
			DrawTextureRepositoryTree();
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
				if (GUILayout.Button(spriteDescription.FileName, GUI.skin.label)) _selectedSpriteDescription = spriteDescription;
				GUI.color = Color.white;
				EditorGUILayout.EndHorizontal();
			}
		}

		private bool CheckForSelection(SpriteDescription spriteDescription)
		{
			return spriteDescription == _selectedSpriteDescription;
		}
	}
}