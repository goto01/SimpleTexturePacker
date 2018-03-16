using System.Collections.Generic;
using UnityEngine;

namespace Editor.TexturePacker.Repository
{
	public class TextureRepository : ScriptableObject
	{
		public Folder Root = new Folder(){Name = "Root"};
	}
}