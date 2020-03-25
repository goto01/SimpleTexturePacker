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
		public const string DestinationTexturePath = "Destination Texture Path";
		public const string DestinationDataPath = "Destination Data Path";
		public const string SourceFolder = "Source Folder";
		public const string ShapePadding = "Shape Padding";
		public const string Extrude = "Extrude";
		
		public static string Publish(string texturePackerPath, string destinationTexturePath, string destinationDataPath, 
			string sourceFolder, int shapePadding, int extrude)
		{
			var args = string.Format("--extrude {5} " +
			                         "--algorithm Basic " +
			                         "--trim-mode None " +
			                         "--png-opt-level 0 " +
			                         "--shape-padding {4} " +
			                         "--disable-auto-alias " +
			                         "--sheet  \"{0}{1}\" " +
			                         "--data \"{0}{2}\" " +
			                         "--format json-array \"{3}\"",
				Application.dataPath, destinationTexturePath, destinationDataPath, sourceFolder, shapePadding, extrude);
			UnityEngine.Debug.Log(args);
			Process.Start(texturePackerPath, args);
			return args;
		}

		public static string Publish()
		{
			return Publish(EditorPrefs.GetString(TexturePackerPath), EditorPrefs.GetString(DestinationTexturePath), 
				EditorPrefs.GetString(DestinationDataPath), EditorPrefs.GetString(SourceFolder), EditorPrefs.GetInt(ShapePadding),
				EditorPrefs.GetInt(Extrude));
		}
	}
}