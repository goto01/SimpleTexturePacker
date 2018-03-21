using System.Collections.Generic;
using System.Linq;
using System.Text;
using TexturePacker.Editor.Domain;
using TexturePacker.Editor.Domain.Entities;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Transformation
{
	public static class Transformation
	{
		private static TextureImporter _textureImporter;
		private static TextureDescription _textureDescription;
		private static string _textureAssetPath;
		private static FramesContainer _rawContainer;
		private static List<SpriteDescription> _newSpriteDescriptions;
		private static TextureRepository _textureRepository;
		private static StringBuilder _outputlog;
		
		public static string Transform(TextureDescription textureDescription, TextureRepository textureRepository)
		{
			_outputlog = new StringBuilder();
			_textureRepository = textureRepository;
			_textureDescription = textureDescription;
			_textureAssetPath = AssetDatabase.GetAssetPath(_textureDescription.Texture);
			_rawContainer = Domain.Domain.LoadContainer(_textureDescription.JsonDataFile);
			_newSpriteDescriptions = new List<SpriteDescription>();
			using (var textureImporterWrapper = new TextureImporterWrapper(_textureAssetPath))
			{
				textureImporterWrapper.ClearSpritesMetaData();
				MapRepository(textureImporterWrapper);	
			}
			EditorUtility.SetDirty(_textureDescription);
			SetSprites();
			return _outputlog.ToString();
		}

		private static  void MapRepository(TextureImporterWrapper textureImporterWrapper)
		{
			for (int index = 0; index < _rawContainer.frames.Length; index++)
			{
				var frame = _rawContainer.frames[index];
				var folder = GenerateFolder(frame);
				CreateOrUpdateSprite(folder, frame, textureImporterWrapper);
			}
		}

		private static Folder GenerateFolder(Frame frame)
		{
			var items = frame.filename.Split(TextureRepository.Separator).ToList();
			var depth = items.Count-1;
			var root = _textureRepository.Root;
			for (; items.Count > 1;)
			{
				var folder = TextureRepository.GetFolder(root, items[0]);
				if (folder == null) break;
				items.RemoveAt(0);
				root = folder;
			}
			for (; items.Count > 1;)
			{
				
				_outputlog.Append(' ', (depth-1) * 3);
				_outputlog.AppendLine(string.Format("* Folder created: {0}", items[0]));
				root = TextureRepository.CreateFolder(root, items[0], depth);
				items.RemoveAt(0);
			}
			return root;
		}

		private static  void CreateOrUpdateSprite(Folder folder, Frame frame, TextureImporterWrapper textureImporterWrapper)
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

		private static  SpriteDescription GetSpriteDescription(Folder folder, string spriteFileName)
		{
			return folder.SpriteDescriptions.SingleOrDefault(x => x.FileName.Equals(spriteFileName));
		}

		private static  SpriteDescription CreateSpriteDescription(Folder folder, Frame frame, TextureImporterWrapper textureImporterWrapper)
		{
			_outputlog.Append(' ', folder.Depth * 3);
			_outputlog.AppendLine(string.Format("- Sprite Description created: {0}", frame.filename));
			var spriteDesription = new SpriteDescription(){FileName = frame.filename};
			folder.SpriteDescriptions.Add(spriteDesription);
			CreateSpriteMetaData(frame, textureImporterWrapper);
			return spriteDesription;
		}

		private static  void CreateSpriteMetaData(Frame frame, TextureImporterWrapper textureImporterWrapper)
		{
			var rect = new Rect(frame.frame.x, _textureDescription.Texture.height - frame.frame.y - frame.frame.h, frame.frame.w, frame.frame.h);
			var pivot = new Vector2(frame.pivot.x, frame.pivot.y);
			textureImporterWrapper.AddSpriteMetaData(frame.filename, rect, pivot);
		}

		private static  void SetSprites()
		{
			_outputlog.AppendLine();
			_outputlog.AppendLine("*** New sprites ***");
			_outputlog.AppendLine();
			
			using (var textureImporterWrapper = new TextureImporterWrapper(_textureAssetPath, false))
			{
				foreach (var spriteDescription in _newSpriteDescriptions)
				{
					spriteDescription.Sprite = textureImporterWrapper.GetSprite(spriteDescription.FileName);
					_outputlog.AppendLine(spriteDescription.FileName);
				}
			}
		}
	}
}