using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AzureSuiteForUnity.CognitiveServices
{
    /// <summary>
    /// Holding class for all those cognitive services keys
    /// </summary>
    [Serializable]
    public class CognitiveServices
    {
        public CognitiveService ComputerVisionAPI { get; set; }
        //public Service LUIS { get; set; }
        //public Service FaceAPI { get; set; }
        //public Service EmotionAPI { get; set; }
        //public Service BingSearchAPI { get; set; }
        public CognitiveService BingSpeechAPI { get; set; }
        //public Service BingSpellCheckAPI { get; set; }
        //public Service ContentModerator { get; set; }
        //public Service CustomSpeechService { get; set; }
        //public Service RecommendationAPI { get; set; }
        //public Service TextAnalyticsAPI { get; set; }
        //public Service TranslatorTextAPI { get; set; }
    }

    public class CognitiveService
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public Type ServiceBehaviour { get; set; }
    }


    public class CognitiveServicesKeyManager : SingletonBehaviour<CognitiveServicesKeyManager>
    {
        private const string TITLE_BING_SPEECH_API = "Bing Speech API";
        private const string TITLE_COMPUTER_VISION_API = "Computer Vision API";

        public CognitiveServices Services { get; set; }

        // Use this for initialization
        public new void Awake()
        {
            // It's a singleton behaviour, and we're overriding awake
            // so ensure the initialisation occurs.
            base.Awake();

            //// Configure the services
            //Services = new CognitiveServices()
            //{
            //    BingSpeechAPI = new CognitiveService()
            //    {
            //        Key = String.Empty,
            //        Name = TITLE_BING_SPEECH_API,
            //        ServiceBehaviour = typeof(BingSpeechAPIManager)
            //    },
            //    ComputerVisionAPI = new CognitiveService()
            //    {
            //        Key = String.Empty,
            //        Name = TITLE_COMPUTER_VISION_API,
            //        ServiceBehaviour = typeof(ComputerVisionAPIManager)
            //    }
            //};
        }

        public void OnEditorGUI()
        {

        }
    }

}