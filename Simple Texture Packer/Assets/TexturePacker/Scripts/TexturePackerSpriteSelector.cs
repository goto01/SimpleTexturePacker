using UnityEngine;

namespace TexturePacker
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class TexturePackerSpriteSelector : MonoBehaviour
	{
		[SerializeField] private SpriteRenderer _spriteRenderer;

		private void Reset()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}
}