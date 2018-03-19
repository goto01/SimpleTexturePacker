using System;
using Editor.TexturePacker.DialogWindows;
using Editor.TexturePacker.Domain;
using Editor.TexturePacker.Repository;
using Editor.Windows.DialogWindows;
using TexturePacker.Domain;
using UnityEditor;

namespace Editor
{
	public static class MenuItems
	{
		private const string CreateMenu = "Assets/Create/Texture packer";
		
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
	}
}