using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holding class for all those cognitive services keys
/// </summary>
[Serializable]
public class CognitiveServicesKeyList
{
    public string ComputerVisionAPI;
    public string LUIS;
    public string FaceAPI;
    public string EmotionAPI;
    public string BingSearchAPI;
    public string BingSpeechAPI;
    public string BingSpellCheckAPI;
    public string ContentModerator;
    public string CustomSpeechService;
    public string RecommendationAPI;
    public string TextAnalyticsAPI;
    public string TranslatorTextAPI;
}

public class CognitiveServicesKeyManager : SingletonBehaviour<CognitiveServicesKeyManager>
{
    public CognitiveServicesKeyList Keys;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
