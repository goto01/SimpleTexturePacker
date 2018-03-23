using System;
using UnityEngine;

namespace TexturePacker.Editor.Repository
{
	[Serializable]
	public class SpriteDescription
	{
		public Sprite Sprite;
		public string FileName;
		public Vector2 Pivot;

		public float Height;
	}
}