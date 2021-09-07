using UnityEngine;

namespace TexturePacker.Editor.Publishing
{
	public class PublishDescription : ScriptableObject
	{
		public string DestinationTexturePath;
		public string DestinationDataPath;
		public string SourceFolder;
		public int ShapePadding;
		public int Extrude;
		public int FixedWidth;
		public int FixedHeight;
	}
}