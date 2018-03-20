using System;
using System.Collections.Generic;

namespace Editor.TexturePacker.Repository
{
	[Serializable]
	public class Folder
	{
		public string Name;
		public int Depth;
		public List<Folder> Folders = new List<Folder>();
		public List<SpriteDescription> SpriteDescriptions = new List<SpriteDescription>();

		public bool Collapsed;
	}
}