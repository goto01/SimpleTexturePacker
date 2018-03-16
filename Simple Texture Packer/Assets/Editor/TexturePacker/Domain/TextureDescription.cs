﻿using Editor.TexturePacker.Repository;
using UnityEngine;

namespace Editor.TexturePacker.Domain
{
	public class TextureDescription : ScriptableObject
	{
		public string Name;
		public TextAsset JsonDataFile;
		public Texture2D Texture;
		public TextureRepository TextureRepository;
	}
}