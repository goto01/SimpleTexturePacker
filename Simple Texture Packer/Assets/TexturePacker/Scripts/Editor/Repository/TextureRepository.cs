using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TexturePacker.Editor.Repository
{
	public class TextureRepository : ScriptableObject
	{
		public const char Separator = '/';
		
		public Folder Root = new Folder(){Name = "Root"};

		public Folder GetFolder(string path)
		{
			var items = path.Split(TextureRepository.Separator).ToList();
			var folder = Root;
			for (var index = 0; index < items.Count; index++)
			{
				folder = GetFolder(folder, items[index]);
				if (folder == null) return null;
			}
			return folder;
		}

		public List<Sprite> GetSpritesOfFolder(string path)
		{
			var folder = GetFolder(path);
			if (folder == null) return null;
			return folder.SpriteDescriptions.Select(x => x.Sprite).ToList();
		}
		
		public static Folder GetFolder(Folder folder, string folderName)
		{
			return folder.Folders.SingleOrDefault(x => x.Name.Equals(folderName));
		}

		public static  Folder CreateFolder(Folder parent, string folderName, int depth)
		{
			var folder = new Folder() {Name = folderName, Depth = depth};
			parent.Folders.Add(folder);
			return folder;
		}
	}
}