using Editor.TexturePacker.DialogWindows;
using Editor.TexturePacker.Domain;
using Editor.TexturePacker.Repository;
using Editor.TexturePacker.Windows;
using Editor.Windows.DialogWindows;
using UnityEditor;

namespace Editor
{
	public static class MenuItems
	{
		private const string CreateMenu = "Assets/Create/Texture packer";
		private const string TexturePacker = "Texture Packer";
		private const string TexturePackerPublishing = "Texture Packer/Publishing";
		
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
			var textureRepository = ObjectCreatorHelper.CreateAsset<TextureRepository>();
		}

		[MenuItem(TexturePacker + "/Transfomartion Window", false, 0)]
		public static void ShowTransformationWindow()
		{
			TransformationWindow.ShowSelf();
		}

		[MenuItem(TexturePacker + "/Texture Repository Browser Window", false, 0)]
		public static void ShowTextureRepositoryBrowserWindow()
		{
			TextureRepositoryBrowserWindow.ShowSelf();
		}

		[MenuItem(TexturePackerPublishing + "/Publish Settings", false, 0)]
		public static void ShowPublishSettingsWindow()
		{
			PublishSettingsWindow.ShowSelf();
		}

		[MenuItem(TexturePackerPublishing + "/Publish", false, 0)]
		public static void Publish()
		{
			Editor.TexturePacker.Publishing.TexturePackerPublishing.Publish();
		}
	}
}