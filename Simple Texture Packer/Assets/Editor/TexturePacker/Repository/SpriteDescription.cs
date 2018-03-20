using System;
using UnityEngine;

namespace Editor.TexturePacker.Repository
{
	[Serializable]
	public class SpriteDescription
	{
		public Sprite Sprite;
		public string FileName;

		public float Height;
	}
}