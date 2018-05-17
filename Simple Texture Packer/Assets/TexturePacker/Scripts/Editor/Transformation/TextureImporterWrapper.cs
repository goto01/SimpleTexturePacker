using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Transformation
{
	public class TextureImporterWrapper : IDisposable
	{
		private readonly TextureImporter _textureImporter;
		private readonly string _path;
		private readonly List<SpriteMetaData> _spritesMetaData;
		private readonly List<Sprite> _sprites;

		public List<Sprite> Sprites{get { return _sprites; }}
		
		public TextureImporterWrapper(string path, bool resetSprites = true)
		{
			_path = path;
			_textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
			if (resetSprites)
			{
				_textureImporter.spriteImportMode = SpriteImportMode.Single;
				AssetDatabase.ImportAsset(_path, ImportAssetOptions.ForceUpdate);
			}
			_textureImporter.isReadable = true;
			_textureImporter.spriteImportMode = SpriteImportMode.Multiple;
			_spritesMetaData = _textureImporter.spritesheet.ToList();
			_sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().ToList();
		}

		public void ClearSpritesMetaData()
		{
			_spritesMetaData.Clear();
		}

		public void AddSpriteMetaData(string name, Rect rect, Vector2 pivot)
		{
			var smd = new SpriteMetaData()
			{
				alignment = 9,	//Custom pivot
				pivot = pivot,
				rect = rect,
				name = name
			};
			_spritesMetaData.Add(smd);
		}

		public Sprite GetSprite(string name)
		{
			return _sprites.SingleOrDefault(x => x.name.Equals(name));
		}
		
		public void Dispose()
		{
			_textureImporter.spritesheet = _spritesMetaData.ToArray();
			AssetDatabase.ImportAsset(_path, ImportAssetOptions.ForceUpdate);
		}
	}
}