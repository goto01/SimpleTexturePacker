using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Publishing
{
	public static class TexturePackerPublishing
	{
		public const string TexturePackerPath = "Texture Packer Path";
		public const string DestinationTexturePath = "Destination Texture Path";
		public const string DestinationDataPath = "Destination Data Path";
		public const string SourceFolder = "Source Folder";
		
		public static void Publish(string texturePackerPath, string destinationTexturePath, string destinationDataPath, string sourceFolder)
		{
			var args = string.Format("--extrude 0 --algorithm Basic --trim-mode None --png-opt-level 0 --disable-auto-alias --sheet  \"{0}{1}\" --data \"{0}{2}\" --format json-array \"{3}\"",
				Application.dataPath, destinationTexturePath, destinationDataPath, sourceFolder);
			var process = Process.Start(texturePackerPath, args);
		}

		public static void Publish()
		{
			Publish(EditorPrefs.GetString(TexturePackerPath), EditorPrefs.GetString(DestinationTexturePath), EditorPrefs.GetString(DestinationDataPath),
				EditorPrefs.GetString(SourceFolder));
		}
	}
}