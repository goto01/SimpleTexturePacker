using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.TexturePacker.Publishing
{
	public static class TexturePackerPublishing
	{
		public const string TexturePackerPath = "Texture Packer Path";
		public const string DestinationTexturePath = "Destination Texture Path";
		public const string DestinationDataPath = "Destination Data Path";
		public const string SourceFolder = "Source Folder";
		
		public static void Publish(string texturePackerPath, string destinationTexturePath, string destinationDataPath, string sourceFolder)
		{
			var path = Path.GetDirectoryName(Application.dataPath) + "/../Publish.bat";
			var process = Process.Start(path, string.Format("\"{0}\" \"{1}{2}\" \"{1}{3}\" \"{4}\"", 
				texturePackerPath, Application.dataPath, destinationTexturePath, destinationDataPath, sourceFolder));
		}

		public static void Publish()
		{
			Publish(EditorPrefs.GetString(TexturePackerPath), EditorPrefs.GetString(DestinationTexturePath), EditorPrefs.GetString(DestinationDataPath),
				EditorPrefs.GetString(SourceFolder));
		}
	}
}