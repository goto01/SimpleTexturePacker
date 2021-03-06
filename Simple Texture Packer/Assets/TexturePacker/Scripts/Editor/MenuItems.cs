﻿using TexturePacker.Editor.DialogWindows;
using TexturePacker.Editor.Domain;
using TexturePacker.Editor.Repository;
using TexturePacker.Editor.Windows;
using UnityEditor;

namespace TexturePacker.Editor
{
	public static class MenuItems
	{
		private const string CreateMenu = "Assets/Create/Texture packer";
		private const string TexturePacker = "Texture Packer";
		
		[MenuItem(CreateMenu + "/Texture Description", false, 1101)]
		public static void CreateTextureDescription()
		{
			Dialog.ShowDialog<CreateTextureDescriptionDialogWindow>("Create Texture Description Window", DialogType.YesNo)
				.Yes += OnYesCreateTextureDescription;
		}

		private static void OnYesCreateTextureDescription(CreateTextureDescriptionDialogWindow window)
		{
			var textureDescription = ObjectCreatorHelper.CreateAsset<TextureDescription>(window.Name);
			textureDescription.JsonDataFile = window.JsonDataFile;
			textureDescription.Texture = window.Texture2D;
			textureDescription.Name = window.Name;
		}

		[MenuItem(CreateMenu + "/Texture Repository", false, 1102)]
		public static void CreateTextureRepository()
		{
			ObjectCreatorHelper.CreateAsset<TextureRepository>();
		}

		[MenuItem(TexturePacker + "/Transfomartion Window", false, 0)]
		public static void ShowTransformationWindow()
		{
			TransformationWindow.ShowSelf();
		}

		[MenuItem(TexturePacker + "/Texture Repository Browser Window", false, 1)]
		public static void ShowTextureRepositoryBrowserWindow()
		{
			TextureRepositoryBrowserWindow.ShowSelf();
		}

		[MenuItem(TexturePacker + "/Publish", false, 20)]
		public static void Publish()
		{
			Publishing.TexturePackerPublishing.Publish();
		}
	}
}