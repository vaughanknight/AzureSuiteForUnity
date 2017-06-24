using AzureSuiteForUnity.CognitiveServices;
using AzureSuiteForUnity.CognitiveServices.Face;
using AzureSuiteForUnity.CognitiveServices.Face.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Face
{
    public class FaceAPIManager : SingletonBehaviour<FaceAPIManager> //: CognitiveServicesBehaviour<ComputerVisionAPIManager>, ICognitiveServicesBehaviour
    {
        /// <summary>
        /// Face API Key
        /// </summary>
        public string APIKey;
        public bool Verbose = false;
        public event FaceAPIEventHandlers.DetectEventHandler OnDetect;
        public Texture2D TestTexture;
        private long _DetectId { get; set; }
        /// <summary>
        /// When enabled, screen describe requests can be triggered hitting 'X'.
        /// </summary>
        public bool TestMode = false;

        /// <summary>
        /// Computer Vision API Instance
        /// </summary>
        private IFaceAPI _faceAPI;

        new void Awake()
        {
            base.Awake();
            _faceAPI = CognitiveServicesServiceFactory.Instance.GetFaceAPI(APIKey);
            _faceAPI.OnDetect += _faceAPI_OnDetect; 
        }

        /// <summary>
        /// Bubbles up the face API event via the behaviour.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void _faceAPI_OnDetect(object sender, FaceAPIEventArgs args)
        {
            if (Verbose) Debug.LogFormat("Result: {0} faces detected. {1}", args.FacesDetected.Count, Time.timeSinceLevelLoad);
            if (OnDetect != null)
            {
                OnDetect(sender, args);
            }
        }

        /// <summary>
        /// Detect faces in the screen texture
        /// </summary>
        public void DetectScreen()
        {
            _faceAPI.DetectScreenAsync();
        }
        
        /// <summary>
        /// Detect the faces in a Texture2D
        /// </summary>
        /// <param name="texture"></param>
        public void Detect(Texture2D texture)
        {
            _faceAPI.DetectAsync(texture);
        }

        void Update()
        {
            if (TestMode && Input.GetKeyDown(KeyCode.Y))
            {
                _faceAPI.Verbose = Verbose;
                if (Verbose) Debug.Log("TEST MODE: Detect triggered.");
                if(TestTexture != null) Detect(TestTexture);
            }
        }
    }
}
