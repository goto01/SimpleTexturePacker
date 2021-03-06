﻿using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.DialogWindows
{
	public sealed class YesNoDialogWindow : BaseDialogWindow<YesNoDialogWindow>
	{
		[SerializeField] private string _message;

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}
		
		protected override void DrawContentEditor()
		{
			EditorGUILayout.LabelField(_message);
		}
	}
}