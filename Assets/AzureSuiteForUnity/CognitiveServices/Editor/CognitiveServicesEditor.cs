using AzureSuiteForUnity.CognitiveServices;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CognitiveServicesEditor : Editor {
    protected const string IMAGE_FOLDER = @"Assets/AzureCognitiveServicesForUnity/Services/Editor/Images/";

    private const string ICON_FILE = "computer_vision_api_icon.png";

    private static Texture2D _logo;
    public static Texture2D Logo
    {
        get
        {
            if (_logo == null)
            {
                _logo = AssetDatabase.LoadAssetAtPath<Texture2D>(IMAGE_FOLDER + ICON_FILE);
            }
            return _logo;
        }
    }

    private bool _otherVisible = false;
    private bool _showKey = false;

    private ICognitiveServicesBehaviour _target;

    public override void OnInspectorGUI()
    {
        _target = (ICognitiveServicesBehaviour)target;

        GUILayout.BeginHorizontal();
        GUILayout.Label(Logo, GUILayout.Height(InspectorSettings.ICON_HEIGHT), GUILayout.Width(InspectorSettings.ICON_WIDTH));
        GUILayout.BeginVertical();
        GUILayout.Space(InspectorSettings.TITLE_FONT_PADDING_TOP);
        RenderTitle();
        RenderAPIKeyField();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        RenderExtendedSettings();
    }

    private void RenderExtendedSettings()
    {
        _otherVisible = EditorGUILayout.Foldout(_otherVisible, "Other Settings / Information");

        if (_otherVisible)
        {
            GUILayout.Label("Token", EditorStyles.boldLabel);
            if (string.IsNullOrEmpty(_target.Token))
            {
                GUILayout.Label("No token.");
            }
            if (!string.IsNullOrEmpty(_target.Token))
            {
                GUILayout.TextArea(_target.Token);
                if (GUILayout.Button("Clear Token"))
                {
                    _target.Token = String.Empty;
                }
            }
        }
    }

    private void RenderAPIKeyField()
    {
        GUILayout.Label("API Key", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();


        if (_showKey)
        {
            _target.APIKey = EditorGUILayout.TextField(_target.APIKey);
        }
        else
        {
            _target.APIKey = EditorGUILayout.PasswordField(_target.APIKey);
        }

        _showKey = GUILayout.Toggle(_showKey, "Show");

        GUILayout.EndHorizontal();

        if (string.IsNullOrEmpty(_target.APIKey) || _target.APIKey.Length != 32)
        {
            GUILayout.Label("The API key should be 32 characters long.", EditorStyles.boldLabel);
        }
    }

    private void RenderTitle()
    {
        var s = GUI.skin.label.fontSize;
        GUI.skin.label.fontSize = InspectorSettings.TITLE_FONT_SIZE;
        GUILayout.Label(_target.Title);
        GUI.skin.label.fontSize = s;
    }
}
