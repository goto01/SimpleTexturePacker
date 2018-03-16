using Editor.TexturePacker.Domain;
using Editor.TexturePacker.Repository;
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
			ObjectCreatorHelper.CreateAsset<TextureDescription>();
		} 
		
		[MenuItem(CreateMenu + "/Texture Repository", false, 1102)]
		public static void CreateTextureRepository()
		{
			var textureRepository = ObjectCreatorHelper.CreateAsset<TextureRepository>();
		} 
	}
}