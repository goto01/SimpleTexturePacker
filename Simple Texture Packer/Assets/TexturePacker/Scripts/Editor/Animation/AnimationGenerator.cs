using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Animation
{
	public static class AnimationGenerator
	{
		public static AnimationClip GenerateAndSaveAnimation(List<Sprite> sprites, int frameRate, string name, string path)
		{
			var animationClip = GenerateAnimation(sprites, frameRate, name);
			AssetDatabase.CreateAsset(animationClip, path);
			AssetDatabase.SaveAssets();
			return animationClip;
		}

		public static AnimationClip GenerateAnimation(List<Sprite> sprites, int frameRate, string name)
		{
			var animationClip = new AnimationClip()
			{
				name = name,
				frameRate = frameRate,
			};
			var editorCurveBinding = new EditorCurveBinding()
			{
				type = typeof(SpriteRenderer),
				path = string.Empty,
				propertyName = "m_Sprite",
			};
			var timeStep = 1f / frameRate;
			var keyFrames = new ObjectReferenceKeyframe[sprites.Count];
			for (var index = 0; index < sprites.Count; index++)
			{
				keyFrames[index] = new ObjectReferenceKeyframe()
				{
					time = timeStep * index,
					value = sprites[index],
				};
			}
			AnimationUtility.SetObjectReferenceCurve(animationClip, editorCurveBinding, keyFrames);
			animationClip.EnsureQuaternionContinuity();
			return animationClip;
		}
	}
}