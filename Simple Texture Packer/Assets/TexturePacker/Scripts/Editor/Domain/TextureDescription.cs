using UnityEngine;

namespace TexturePacker.Editor.Domain
{
	public class TextureDescription : ScriptableObject
	{
		public string Name;
		public string TransformationDate;
		public TextAsset JsonDataFile;
		public Texture2D Texture;
	}
}