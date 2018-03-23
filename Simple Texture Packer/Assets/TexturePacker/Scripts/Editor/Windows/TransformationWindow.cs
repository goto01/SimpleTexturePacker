using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TexturePacker.Editor.DialogWindows;
using TexturePacker.Editor.Domain;
using TexturePacker.Editor.Publishing;
using TexturePacker.Editor.Repository;
using UnityEditor;
using UnityEngine;

namespace TexturePacker.Editor.Windows
{
	public class TransformationWindow : EditorWindow
	{
		private const int DateLabelWidth = 150;
		private const int DateSeparatorWidth = 20;
		private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
		private readonly Color _alertColor = new Color(1, .4f, .4f);
		private readonly Color _alrightColor = new Color(.4f, 1, .4f);
		
		private List<TextureDescription> _textureDescriptions;
		private List<TextureRepository> _textureRepositories;
		private string[] _textureRepositoryNames;
		private TextureRepository _targetTextureRepository;
		private string _outputlog;
		private Vector2 _outputlogScroll;

		public static void ShowSelf()
		{
			EditorWindow.GetWindow<TransformationWindow>(false, "Texture Transfomation Window");
		}
		
		protected virtual void OnGUI()
		{
			Init();
			DrawHeaderInspector();
			if (_targetTextureRepository != null)
			{
				DrawTextureDescriptionsInspector();
				EditorGUILayout.Space();
			}
			else
			{
				EditorGUILayout.HelpBox("Select target Texture Repository or create new one", MessageType.Warning);
			}
			DrawTargetRepositoryInspector();
			DrawOutputlogEditor();
		}

		private void Init()
		{
			if (_textureDescriptions == null) InitTextureDescriptions();
			if (_textureRepositories == null) InitTextureRepositories();
			if (_outputlog == null) _outputlog = string.Empty;
		}

		private void InitTextureDescriptions()
		{
			_textureDescriptions = AssetDatabaseHelper.LoadAllAssets<TextureDescription>();
		}

		private void InitTextureRepositories()
		{
			_textureRepositories = AssetDatabaseHelper.LoadAllAssets<TextureRepository>();
			if (_textureRepositories.Count > 0)
			{
				_targetTextureRepository = _textureRepositories[0];
				_textureRepositoryNames = _textureRepositories.Select(x => x.name).ToArray();
			}
		}

		private void DrawHeaderInspector()
		{
			EditorGUILayout.LabelField("Texture transformation", EditorStyles.boldLabel);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Refresh", EditorStyles.miniButtonLeft))
			{
				InitTextureDescriptions();
				InitTextureRepositories();
			}
			if (GUILayout.Button("Publish", EditorStyles.miniButtonRight, GUILayout.Width(100))) TexturePackerPublishing.Publish();
			EditorGUILayout.EndHorizontal();
		}
		
		private void DrawTextureDescriptionsInspector()
		{
			foreach (var textureDescription in _textureDescriptions)
			{
				EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(position.width - EditorGUIUtility.standardVerticalSpacing*3));
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(textureDescription.Name);
				
				DrawTimeInspector(textureDescription);
				if (GUILayout.Button("Transform to repository", EditorStyles.miniButtonLeft))
				{
					var window = Dialog.ShowDialog<YesNoDialogWindow>("Transform to texture repository", DialogType.YesNo);
					window.Message = string.Format("Transform Texture Description {0} to Texture Repository {1}", textureDescription.Name,
						_targetTextureRepository.name);
					var td = textureDescription;
					window.Yes += sender => TransformTextureDescription(td);
				}
				if (GUILayout.Button("Select", EditorStyles.miniButtonRight))
				{
					EditorGUIUtility.PingObject(textureDescription);
					Selection.activeObject = textureDescription;
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
		}

		private void DrawTimeInspector(TextureDescription textureDescription)
		{
			var filePath = AssetDatabase.GetAssetPath(textureDescription.JsonDataFile);
			var lastWriteDate = File.GetLastWriteTime(filePath);
			EditorGUILayout.LabelField(lastWriteDate.ToString(DateTimeFormat), GUILayout.Width(DateLabelWidth));
			if (string.IsNullOrEmpty(textureDescription.TransformationDate))
			{
				EditorGUILayout.LabelField(string.Empty, GUILayout.Width(DateSeparatorWidth));
				EditorGUILayout.LabelField("Need to transform", GUILayout.Width(DateLabelWidth));
				return;
			}
			var transformationDate = DateTime.Parse(textureDescription.TransformationDate);
			if (DateTime.Compare(lastWriteDate, transformationDate) > 0)
			{
				GUI.color = _alertColor;
				EditorGUILayout.LabelField(">", GUILayout.Width(DateSeparatorWidth));
			}
			else
			{
				GUI.color = _alrightColor;
				EditorGUILayout.LabelField("<", GUILayout.Width(DateSeparatorWidth));
			}
			
			EditorGUILayout.LabelField(textureDescription.TransformationDate, GUILayout.Width(DateLabelWidth));
			GUI.color = Color.white;
		}

		private void TransformTextureDescription(TextureDescription textureDescription)
		{
			textureDescription.TransformationDate = DateTime.Now.ToString(DateTimeFormat);
			_outputlog = Transformation.Transformation.Transform(textureDescription, _targetTextureRepository);
			EditorUtility.SetDirty(textureDescription);
			EditorUtility.SetDirty(_targetTextureRepository);
		}

		private void DrawTargetRepositoryInspector()
		{
			if (_textureRepositories.Count == 0)
			{
				EditorGUILayout.HelpBox("You don't have any Texture Repository instance", MessageType.Error);
				return;
			}
			var index = _textureRepositories.IndexOf(_targetTextureRepository);
			index = EditorGUILayout.Popup("Target texture repository", index, _textureRepositoryNames);
			_targetTextureRepository = _textureRepositories[index];
		}

		private void DrawOutputlogEditor()
		{
			_outputlogScroll = EditorGUILayout.BeginScrollView(_outputlogScroll, GUI.skin.box);
			EditorGUILayout.LabelField(_outputlog, GUILayout.Height(_outputlog.Count(x=>x.Equals('\n')) * EditorGUIUtility.singleLineHeight));
			EditorGUILayout.EndScrollView();
		}
	}
}