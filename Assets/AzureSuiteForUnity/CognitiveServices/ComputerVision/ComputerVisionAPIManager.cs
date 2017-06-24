using AzureSuiteForUnity.CognitiveServices.ComputerVision;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.ComputerVision
{
    /// <summary>
    /// This class abstracts the FaceAPI interactions from the Unity behaviours.
    /// Gets all the Unity code out of the API, and all the API code out of the behaviour.
    /// </summary>
    public class ComputerVisionAPIManager : SingletonBehaviour<ComputerVisionAPIManager> //: CognitiveServicesBehaviour<ComputerVisionAPIManager>, ICognitiveServicesBehaviour
    {
        /// <summary>
        /// Computer Vision API Key
        /// </summary>
        public string APIKey;
        public bool Verbose = false;
        public event ComputerVisionAPIEventHandlers.DescribeEventHandler OnDescribe;

        /// <summary>
        /// When enabled, screen describe requests can be triggered hitting 'X'.
        /// </summary>
        public bool TestMode = false;

        /// <summary>
        /// Computer Vision API Instance
        /// </summary>
        private IComputerVisionAPI _computerVisionAPI;

        void Start()
        {
            _computerVisionAPI = CognitiveServicesServiceFactory.Instance.GetComputerVisionAPI(APIKey);
            _computerVisionAPI.OnDescribe += _computerVisionAPI_OnDescribe;
            //APIKey = "d0584ebec5524eafbf5f2d8ca4fc0ae5";
        }

        /// <summary>
        /// Describe the screen
        /// </summary>
        public void DescribeScreen()
        {
            _computerVisionAPI.DescribeScreenAsync();
        }


        /// <summary>
        /// Hooks the FaceAPI OnDescribe event and bubbles it up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="result"></param>
        private void _computerVisionAPI_OnDescribe(object sender, string result)
        {
            if(Verbose) Debug.LogFormat("Result is {0}", result);
            if(OnDescribe != null)
            {
                OnDescribe(sender, result);
            }
        }

        /// <summary>
        /// Describe a Texture2D
        /// </summary>
        /// <param name="texture"></param>
        public void Describe(Texture2D texture)
        {
            _computerVisionAPI.DescribeAsync(texture);
        }
        
        void Update()
        {                   
            if (TestMode && Input.GetKeyDown(KeyCode.X))
            {
                _computerVisionAPI.Verbose = Verbose;
                if (Verbose) Debug.Log("TEST MODE: DescribeScreen triggered.");
                DescribeScreen(); 
            }
        }
    }
}