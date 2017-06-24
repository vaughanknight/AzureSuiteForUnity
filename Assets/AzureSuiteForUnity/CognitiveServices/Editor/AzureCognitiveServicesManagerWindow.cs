using AzureSuiteForUnity.CognitiveServices;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AzureCognitiveServicesManagerWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    private const string IMAGE_FOLDER = @"Assets/AzureCognitiveServicesForUnity/Core/Images";
    private static Texture2D _logo;

    private Texture2D Logo
    {
        get
        {
            if(_logo == null)
            {
                _logo = AssetDatabase.LoadAssetAtPath<Texture2D>(IMAGE_FOLDER + @"/title_image.psd");
            }
            return _logo;
        }
    }
    // Add menu named "My Window" to the Window menu
    [MenuItem("Azure/Cognitive Services Manager")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (AzureCognitiveServicesManagerWindow)EditorWindow.GetWindow(typeof(AzureCognitiveServicesManagerWindow), false, "Azure Cognitive Services");
        window.Show();
    }

    private CognitiveServicesKeyManager _keyManager;

    void OnGUI()
    {
        GUI.backgroundColor = Color.yellow;
        GUILayout.Label(Logo, GUILayout.Height(100), GUILayout.Width(500));

        _keyManager = (CognitiveServicesKeyManager)GameObject.FindObjectOfType(typeof(CognitiveServicesKeyManager));
        if (_keyManager == null)
        {
            RenderManagerDoesNotExist();
        }
        else
        {
            RenderManagerExists();
        }
    }

    void RenderManagerDoesNotExist()
    {
        GUILayout.Label("Key manager does not exist.");
        if (GUILayout.Button("Create Key Manager", GUILayout.Width(200), GUILayout.Height(30)))
        {
            var go = new GameObject("AzureCognitiveServices");
            go.AddComponent<CognitiveServicesKeyManager>();
        }
    }

    void RenderManagerExists()
    {
        GUILayout.Label("Enable Services", EditorStyles.boldLabel);

        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        //AddServiceToggle<BingSpeechAPIManager>("Bing Speech Support", _keyManager.Keys.BingSpeechAPI);
        
        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();
    }

    private void AddServiceToggle<T>(string name, string key) where T : MonoBehaviour
    {
        var api = _keyManager.GetComponent<T>();
        if (api == null)
        {
            if (GUILayout.Button("Add " + name, GUILayout.Width(200), GUILayout.Height(30)))
            {
                _keyManager.gameObject.AddComponent<T>();
            }
        }
        else
        {
            if (GUILayout.Button("Remove " + name, GUILayout.Width(200), GUILayout.Height(30)))
            {
                GameObject.DestroyImmediate(api);
            }
            if (System.String.IsNullOrEmpty(key))
            {
                ShowWarning(name);
            }
        }
    }

    public void ShowWarning(string name)
    {
        var c = GUI.backgroundColor;
        GUI.backgroundColor = Color.red;
        GUILayout.Label("WARNING: No key set for " + name, EditorStyles.boldLabel);
        GUI.backgroundColor = c;
    }
}
