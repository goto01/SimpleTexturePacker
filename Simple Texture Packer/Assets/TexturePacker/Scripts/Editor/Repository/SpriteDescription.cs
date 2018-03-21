using System;
using UnityEngine;

namespace TexturePacker.Editor.Repository
{
	[Serializable]
	public class SpriteDescription
	{
		public Sprite Sprite;
		public string FileName;

		public float Height;
	}
}