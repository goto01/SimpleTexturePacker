using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace TexturePacker.Editor.Publishing
{
	public static class TexturePackerPublishing
	{
		public const string TexturePackerPath = "Texture Packer Path";
		
		public static string Publish(string texturePackerPath, string destinationTexturePath, string destinationDataPath, 
			string sourceFolder, int shapePadding, int extrude, int width, int height)
		{
			var args = string.Format("--extrude {5} " +
			                         "--width {6} " +
			                         "--height {7} " +
			                         "--algorithm Basic " +
			                         "--trim-mode None " +
			                         "--png-opt-level 0 " +
			                         "--shape-padding {4} " +
			                         "--disable-auto-alias " +
			                         "--sheet  \"{0}{1}\" " +
			                         "--data \"{0}{2}\" " +
			                         "--format json-array \"{3}\"",
				Application.dataPath, destinationTexturePath, destinationDataPath, sourceFolder, shapePadding, extrude,
				width, height);
			UnityEngine.Debug.Log(args);
			Process.Start(texturePackerPath, args);
			return args;
		}
		public static string Publish(PublishDescription publishDescription)
		{
			return Publish(EditorPrefs.GetString(TexturePackerPath),
				publishDescription.DestinationTexturePath,
				publishDescription.DestinationDataPath,
				publishDescription.SourceFolder,
				publishDescription.ShapePadding,
				publishDescription.Extrude,
				publishDescription.FixedWidth,
				publishDescription.FixedHeight);
		}
	}
}