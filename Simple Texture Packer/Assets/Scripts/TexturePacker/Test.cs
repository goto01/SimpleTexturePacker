using System.IO;
using TexturePacker.Domain.Entities;
using UnityEngine;

namespace TexturePacker
{
	public class Test : MonoBehaviour
	{
		protected virtual void Start()
		{
			var jsonData = File.ReadAllText(@"d:/Soft/Texture packer/bin/Examples/texting.json");
			var container = JsonUtility.FromJson<FramesContainer>(jsonData);
		}
	}
}