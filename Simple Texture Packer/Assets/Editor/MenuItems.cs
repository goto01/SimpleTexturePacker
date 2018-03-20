using Editor.TexturePacker.DialogWindows;
using Editor.TexturePacker.Domain;
using Editor.TexturePacker.Publishing;
using Editor.TexturePacker.Repository;
using Editor.TexturePacker.Windows;
using Editor.Windows.DialogWindows;
using UnityEditor;
using UnityEditor.LinuxStandalone;

namespace Editor
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

		[MenuItem(TexturePacker + "/Publish", false, 0)]
		public static void Publish()
		{
			TexturePackerPublishing.Publish("d:/Soft/Texture packer/bin/TexturePacker", "/Textures/texting{n}.png",
				"/Textures/texting{n}.json", "d:/Unity/SimpleTexturePacker/Characters/");
		}
	}
}