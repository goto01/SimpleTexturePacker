using System;
using System.Collections.Generic;

namespace TexturePacker.Editor.Repository
{
	[Serializable]
	public class Folder
	{
		public string Name;
		public int Depth;
		public List<Folder> Folders = new List<Folder>();
		public List<SpriteDescription> SpriteDescriptions = new List<SpriteDescription>();
		public int FrameRate;
		public bool Loop;

		public bool Collapsed;
	}
}