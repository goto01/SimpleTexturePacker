using TexturePacker.Properties;
using UnityEngine;

namespace TexturePacker
{
	public class TestBehaviour : MonoBehaviour
	{
		[SerializeField] [SpriteProperty] private Sprite _sprite;
		[SerializeField] private float _value;
	}
}