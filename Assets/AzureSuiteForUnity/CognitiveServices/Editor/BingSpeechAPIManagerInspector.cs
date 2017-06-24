using AzureSuiteForUnity.CognitiveServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(BingSpeechAPIManager))]
public class BingSpeechAPIManagerInspector : CognitiveServicesEditor
{
    //private const string ICON_FILE = "bing_speech_api_icon.png";

    //private static Texture2D _logo;
    //public static Texture2D Logo
    //{
    //    get
    //    {
    //        if (_logo == null)
    //        {
    //            _logo = AssetDatabase.LoadAssetAtPath<Texture2D>(IMAGE_FOLDER + ICON_FILE);
    //        }
    //        return _logo;
    //    }
    //}
    
    //private bool _otherVisible = false;
    //private bool _showKey = false;

    //private BingSpeechAPIManager _target;

    //public override void OnInspectorGUI()
    //{
    //    _target = (BingSpeechAPIManager)target;

    //    GUILayout.BeginHorizontal();
    //    GUILayout.Label(Logo, GUILayout.Height(100), GUILayout.Width(100));

    //    GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

    //    RenderTitle();
    //    RenderAPIKeyField();

    //    GUILayout.EndVertical();
    //    GUILayout.EndHorizontal();

    //    RenderExtendedSettings();

    //}

    //private void RenderExtendedSettings()
    //{
    //    _otherVisible = EditorGUILayout.Foldout(_otherVisible, "Other Settings / Information");

    //    if (_otherVisible)
    //    {
    //        GUILayout.Label("Token", EditorStyles.boldLabel);
    //        if (string.IsNullOrEmpty(_target.Token))
    //        {
    //            GUILayout.Label("No token.");
    //        }
    //        if (!string.IsNullOrEmpty(_target.Token))
    //        {
    //            GUILayout.TextArea(_target.Token);
    //            if (GUILayout.Button("Clear Token"))
    //            {
    //                _target.Token = String.Empty;
    //            }
    //        }
    //    }
    //}

    //private void RenderAPIKeyField()
    //{
    //    GUILayout.Label("API Key", EditorStyles.boldLabel);

    //    GUILayout.BeginHorizontal();

    //    if (_showKey)
    //    {
    //        _target.APIKey = EditorGUILayout.TextField(_target.APIKey);
    //    }
    //    else
    //    {
    //        _target.APIKey = EditorGUILayout.PasswordField(_target.APIKey);
    //    }

    //    _showKey = GUILayout.Toggle(_showKey, "Show");

    //    GUILayout.EndHorizontal();

    //    if (string.IsNullOrEmpty(_target.APIKey) || _target.APIKey.Length != 32)
    //    {
    //        GUILayout.Label("The API key should be 32 characters long.", EditorStyles.boldLabel);
    //    }
    //}

    //private static void RenderTitle()
    //{
    //    var s = GUI.skin.label.fontSize;
    //    GUI.skin.label.fontSize = InspectorSettings.TITLE_FONT_SIZE; ;
    //    GUILayout.Label("Bing Speech API");
    //    GUI.skin.label.fontSize = s;
    //}
}
