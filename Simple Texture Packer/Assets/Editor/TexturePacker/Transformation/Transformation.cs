using System.Linq;
using Editor.TexturePacker.Domain;
using Editor.TexturePacker.Domain.Entities;
using Editor.TexturePacker.Repository;
using UnityEngine;

namespace Editor.TexturePacker.Transformation
{
	public class Transformation
	{
		private const char Separator = '/';

		public static void Transform(TextureDescription textureDescription)
		{
			var rawContainer = Domain.Domain.LoadContainer(textureDescription.JsonDataFile);
			MapRepository(rawContainer, textureDescription.TextureRepository);
		}

		private static void MapRepository(FramesContainer rawContainer, TextureRepository textureRepository)
		{
			for (int index = 0; index < rawContainer.frames.Length; index++)
			{
				MapFrame(rawContainer.frames[index], textureRepository);
			}
		}

		private static void MapFrame(Frame frame, TextureRepository textureRepository)
		{
			var items = frame.filename.Split(Separator).ToList();
			var root = textureRepository.Root;
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
			CreateOrUpdateSprite(root, frame);
		}

		private static void CreateOrUpdateSprite(Folder folder, Frame frame)
		{
			var spriteDescription = GetSpriteDescription(folder, frame.filename);
			if (spriteDescription == null) spriteDescription = CreateSpriteDescription(folder, frame.filename);
			
		}

		private static Folder GetFolder(Folder folder, string folderName)
		{
			return folder.Folders.SingleOrDefault(x => x.Name.Equals(folderName));
		}

		private static Folder CreateFolder(Folder parent, string folderName)
		{
			var folder = new Folder() {Name = folderName};
			parent.Folders.Add(folder);
			return folder;
		}

		private static SpriteDescription GetSpriteDescription(Folder folder, string spriteFileName)
		{
			return folder.SpriteDescriptions.SingleOrDefault(x => x.FileName.Equals(spriteFileName));
		}

		private static SpriteDescription CreateSpriteDescription(Folder folder, string spriteFileName)
		{
			var spriteDesription = new SpriteDescription(){FileName = spriteFileName};
			folder.SpriteDescriptions.Add(spriteDesription);
			return spriteDesription;
		}
	}
}