using System.Collections.Generic;
using UnityEditor;

namespace TexturePacker.Editor
{
	public static class AssetDatabaseHelper
	{
		public static List<T> LoadAllAssets<T>() where T : UnityEngine.Object
		{
			var list = new List<T>();
			var assetGuids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T).Name));
			foreach (var assetGuid in assetGuids)
			{
				var path = AssetDatabase.GUIDToAssetPath(assetGuid);
				list.Add(AssetDatabase.LoadAssetAtPath<T>(path));
			}
			return list;
		}
	}
}