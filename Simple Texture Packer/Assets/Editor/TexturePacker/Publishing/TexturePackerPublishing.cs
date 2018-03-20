﻿using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Editor.TexturePacker.Publishing
{
	public static class TexturePackerPublishing
	{
		public static void Publish(string texturePackerPath, string destinationTexturePath, string destinationDataPath, string sourceFolder)
		{
			var path = Path.GetDirectoryName(Application.dataPath) + "/../Publish.bat";
			var process = Process.Start(path, string.Format("\"{0}\" \"{1}{2}\" \"{1}{3}\" \"{4}\"", 
				texturePackerPath, Application.dataPath, destinationTexturePath, destinationDataPath, sourceFolder));
		}
	}
}