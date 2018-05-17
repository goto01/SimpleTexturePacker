using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Animation
{
	public static class AnimationGenerator
	{
		public static AnimationClip GenerateAndSaveAnimation(List<Sprite> sprites, int frameRate, bool isLooping, string path)
		{
			var animationClip = GenerateAnimation(sprites, frameRate, isLooping, Path.GetFileNameWithoutExtension(path));
			if (AssetDatabase.LoadAssetAtPath<AnimationClip>(path) != null) AssetDatabase.DeleteAsset(path);
			AssetDatabase.CreateAsset(animationClip, path);
			AssetDatabase.SaveAssets();
			EditorUtility.SetDirty(animationClip);
			return animationClip;
		}

		public static AnimationClip GenerateAndSaveAnimationToCurrentDirectory(List<Sprite> sprites, int frameRate, bool isLooping, string name)
		{
			var animationClip = GenerateAnimation(sprites, frameRate, isLooping, name);
			ObjectCreatorHelper.CreateAsset(animationClip, string.Format("{0}.anim", name));
			return animationClip;
		}

		private static AnimationClip GenerateAnimation(List<Sprite> sprites, int frameRate, bool isLooping, string name)
		{
			var animationClip = new AnimationClip()
			{
				name = name,
				frameRate = frameRate
			};
			if (isLooping)
			{
				var animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
				animationClipSettings.loopTime = true; 
				AnimationUtility.SetAnimationClipSettings(animationClip, animationClipSettings);
				
			}
			var editorCurveBinding = new EditorCurveBinding()
			{
				type = typeof(SpriteRenderer),
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
			return animationClip;
		}
	}
}