using System;
using System.Collections.Generic;
using TexturePacker.Editor.Repository;
using UnityEngine;

namespace TexturePacker.Editor
{
	public static class TransformationModificators
	{
		//Add modificator method here
		private static readonly List<Action<SpriteDescription>> Modificators = new List<Action<SpriteDescription>>()
		{
			ModificateCharacters,
		}; 
		
		public static void Modificate(SpriteDescription spriteDescription)
		{
			Modificators.ForEach(x=>x(spriteDescription));
		}
		
		//Write modificator methods below

		private static void ModificateCharacters(SpriteDescription spriteDescription)
		{
			if (spriteDescription.FileName.Contains("Characters/"))
			{
				spriteDescription.Pivot = new Vector2(.5f, 0);
			}
		}
	}
}