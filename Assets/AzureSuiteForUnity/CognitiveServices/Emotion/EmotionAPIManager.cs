using AzureSuiteForUnity.CognitiveServices;
using AzureSuiteForUnity.CognitiveServices.Emotion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Emotion
{
    public class EmotionAPIManager : SingletonBehaviour<EmotionAPIManager> //: CognitiveServicesBehaviour<ComputerVisionAPIManager>, ICognitiveServicesBehaviour
    {
        /// <summary>
        /// Emotion API Key
        /// </summary>
        public string APIKey;
        public bool Verbose = false;
        public event EmotionAPIEventHandlers.RecognizeEventHandler OnRecognize;
        public Texture2D TestTexture;
        private long _DetectId { get; set; }
        /// <summary>
        /// When enabled, screen describe requests can be triggered hitting 'X'.
        /// </summary>
        public bool TestMode = false;

        /// <summary>
        /// Emotion API Instance
        /// </summary>
        private IEmotionAPI _emotionAPI;

        new void Awake()
        {
            base.Awake();
            _emotionAPI = CognitiveServicesServiceFactory.Instance.GetEmotionAPI(APIKey);
            _emotionAPI.OnRecognize += _emotionAPI_OnRecognize;
        }

        /// <summary>
        /// Bubbles up the face API event via the behaviour.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void _emotionAPI_OnRecognize(object sender, EmotionAPIEventArgs args)
        {
            if (Verbose) Debug.LogFormat("Result: {0} faces detected. {1}", args.EmotionsDetected.Count, Time.timeSinceLevelLoad);
            if (OnRecognize != null)
            {
                OnRecognize(sender, args);
            }  
        }



        /// <summary>
        /// Detect emotions in the screen texture
        /// </summary>
        public void DetectScreen()
        {
            _emotionAPI.RecognizeScreenAsync();
        }
        
        /// <summary>
        /// Detect the faces in a Texture2D
        /// </summary>
        /// <param name="texture"></param>
        public void Detect(Texture2D texture)
        {
            _emotionAPI.RecognizeAsync(texture);
        }

        void Update()
        {
            if (TestMode && Input.GetKeyDown(KeyCode.Z))
            {
                _emotionAPI.Verbose = Verbose;
                if (Verbose) Debug.Log("TEST MODE: Recognize triggered.");
                if(TestTexture != null) Detect(TestTexture);
            }
        }
    }
}
