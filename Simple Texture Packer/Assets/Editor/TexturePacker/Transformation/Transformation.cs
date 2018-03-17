using System;
using System.Collections.Generic;
using System.Linq;
using Editor.TexturePacker.Domain;
using Editor.TexturePacker.Domain.Entities;
using Editor.TexturePacker.Repository;
using UnityEditor;
using UnityEngine;

namespace Editor.TexturePacker.Transformation
{
	public class Transformation : IDisposable
	{
		private const char Separator = '/';
		private readonly TextureImporter _textureImporter;
		private readonly TextureDescription _textureDescription;
		private readonly string _textureAssetPath;
		private readonly FramesContainer _rawContainer;
		private readonly List<SpriteDescription> _newSpriteDescriptions;
		
		private TextureRepository TextureRepository{get { return _textureDescription.TextureRepository; }}
		
		public Transformation(TextureDescription textureDescription)
		{
			_textureDescription = textureDescription;
			_textureAssetPath = AssetDatabase.GetAssetPath(_textureDescription.Texture);
			_rawContainer = Domain.Domain.LoadContainer(_textureDescription.JsonDataFile);
			_newSpriteDescriptions = new List<SpriteDescription>();
		}
		
		public void Transform()
		{
			using (var textureImporterWrapper = new TextureImporterWrapper(_textureAssetPath))
			{
				textureImporterWrapper.ClearSpritesMetaData();
				MapRepository(textureImporterWrapper);	
			}
			EditorUtility.SetDirty(_textureDescription);
		}

		private void MapRepository(TextureImporterWrapper textureImporterWrapper)
		{
			for (int index = 0; index < _rawContainer.frames.Length; index++)
			{
				var frame = _rawContainer.frames[index];
				var folder = GenerateFolder(frame);
				CreateOrUpdateSprite(folder, frame, textureImporterWrapper);
			}
		}

		private Folder GenerateFolder(Frame frame)
		{
			var items = frame.filename.Split(Separator).ToList();
			var root = TextureRepository.Root;
			for (; items.Count > 1;)
			{
				var folder = GetFolder(root, items[0]);
				if (folder == null) break;
				items.RemoveAt(0);
				root = folder;
			}
			for (; items.Count > 1;)
			{
				root = CreateFolder(root, items[0]);
				items.RemoveAt(0);
			}
			return root;
		}

		private void CreateOrUpdateSprite(Folder folder, Frame frame, TextureImporterWrapper textureImporterWrapper)
		{
			var spriteDescription = GetSpriteDescription(folder, frame.filename);
			if (spriteDescription == null)
			{
				spriteDescription = CreateSpriteDescription(folder, frame, textureImporterWrapper);
				_newSpriteDescriptions.Add(spriteDescription);
				return;
			}
			CreateSpriteMetaData(frame, textureImporterWrapper);
		}

		private Folder GetFolder(Folder folder, string folderName)
		{
			return folder.Folders.SingleOrDefault(x => x.Name.Equals(folderName));
		}

		private Folder CreateFolder(Folder parent, string folderName)
		{
			var folder = new Folder() {Name = folderName};
			parent.Folders.Add(folder);
			return folder;
		}

		private SpriteDescription GetSpriteDescription(Folder folder, string spriteFileName)
		{
			return folder.SpriteDescriptions.SingleOrDefault(x => x.FileName.Equals(spriteFileName));
		}

		private SpriteDescription CreateSpriteDescription(Folder folder, Frame frame, TextureImporterWrapper textureImporterWrapper)
		{
			var spriteDesription = new SpriteDescription(){FileName = frame.filename};
			folder.SpriteDescriptions.Add(spriteDesription);
			CreateSpriteMetaData(frame, textureImporterWrapper);
			return spriteDesription;
		}

		private void CreateSpriteMetaData(Frame frame, TextureImporterWrapper textureImporterWrapper)
		{
			var rect = new Rect(frame.frame.x, _textureDescription.Texture.height - frame.frame.y - frame.frame.h, frame.frame.w, frame.frame.h);
			var pivot = new Vector2(frame.pivot.x, frame.pivot.y);
			textureImporterWrapper.AddSpriteMetaData(frame.filename, rect, pivot);
		}

		public void Dispose()
		{
			using (var textureImporterWrapper = new TextureImporterWrapper(_textureAssetPath, false))
			{
				foreach (var spriteDescription in _newSpriteDescriptions)
				{
					spriteDescription.Sprite = textureImporterWrapper.GetSprite(spriteDescription.FileName);
				}
			}
		}
	}
}