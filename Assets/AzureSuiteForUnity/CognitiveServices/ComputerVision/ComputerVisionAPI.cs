using AzureSuiteForUnity.CognitiveServices.ComputerVision.Model;
using Castle.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace AzureSuiteForUnity.CognitiveServices.ComputerVision
{
    [Interceptor(typeof(LoggingInterceptor))]
    public class ComputerVisionAPI : IComputerVisionAPI
    {
        private struct ComputerVisionStrings 
        {
            public const string URL_DESCRIBE = "https://westus.api.cognitive.microsoft.com/vision/v1.0/describe?";
            public const string QUERY_STRING_DESCRIBE = "maxCandidates=10";
        }

        public event ComputerVisionAPIEventHandlers.DescribeEventHandler OnDescribe;

        public string APIKey { get; set; }
        public bool Verbose { get; set; }
        private MonoBehaviour _asyncBehaviour { get; set; }

        public ComputerVisionAPI(MonoBehaviour behaviour)
        {
            Verbose = false;
            _asyncBehaviour = behaviour;
            //APIKey = "d0584ebec5524eafbf5f2d8ca4fc0ae5";
        }

        public void DescribeScreenAsync()
        {
            _asyncBehaviour.StartCoroutine(DescribeScreen());
        }

        public IEnumerator DescribeScreen()
        {
            //Wait for graphics to render
            yield return new WaitForEndOfFrame();

            //Create a texture to pass to encoding
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            //Put buffer into texture
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

            yield return Describe(texture);
        }

        public void DescribeAsync(Texture2D texture)
        {
            _asyncBehaviour.StartCoroutine(Describe(texture));
        }

        public IEnumerator Describe(Texture2D texture)
        {
            // Get the PNG
            var data = texture.EncodeToPNG();

            // No query customisations yet, but in the future the query string can be 
            // formatted here
            var requestUrl = ComputerVisionStrings.URL_DESCRIBE + ComputerVisionStrings.QUERY_STRING_DESCRIBE;
            if (Verbose) Debug.LogFormat("Request URL {0}.", requestUrl);

            // Add the headers
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(CognitiveStrings.HEADER_OCP_APIM_SUBSCRIPTION_KEY, APIKey);
            headers.Add(CognitiveStrings.HEADER_CONTENT_TYPE, CognitiveStrings.CONTENT_TYPE_OCTET_STREAM);

            // The fantastic Unity WWW request
            var client = new WWW(requestUrl, data, headers);

            yield return client;

            // Dump the response for now
            if (Verbose) Debug.LogFormat("Response {0}.", client.text);

            var obj = JsonConvert.DeserializeObject<DescribeResponse>(client.text);

            if(OnDescribe != null)
            {
                OnDescribe(this, client.text);
            }
        }

        //[Serializable]
        //public class VisionApiResponse
        //{
        //    public Category[] categories;
        //}

        //[Serializable]
        //public class Category
        //{
        //    public string name;
        //    public float score;
        //}
    }
}