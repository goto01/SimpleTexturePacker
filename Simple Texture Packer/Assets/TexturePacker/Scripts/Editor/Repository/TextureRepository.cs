using UnityEngine;

namespace TexturePacker.Editor.Repository
{
	public class TextureRepository : ScriptableObject
	{
		public Folder Root = new Folder(){Name = "Root"};
	}
}