using AzureSuiteForUnity.CognitiveServices.Face.Model;
using Castle.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Face
{
    [Interceptor(typeof(LoggingInterceptor))]
    public class FaceAPI : IFaceAPI
    {
        private struct FaceStrings
        {
            public const string URL_DETECT = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect?";
            public const string QUERY_STRING_FORMAT_DETECT = "returnFaceId={0}&returnFaceLandmarks={1}&returnFaceAttributes={2}";
            public const string PARAMETER_STRING_FACE_ATTRIBUTES = "age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
        }

        public event FaceAPIEventHandlers.DetectEventHandler OnDetect;

        public string APIKey { get; set; }
        public bool Verbose { get; set; }
        private long _requestId { get; set; }
        private MonoBehaviour _asyncBehaviour;

        public FaceAPI(MonoBehaviour behaviour)
        {
            Verbose = false;
            _asyncBehaviour = behaviour;
            //APIKey = "d0584ebec5524eafbf5f2d8ca4fc0ae5";
        }


        public void DetectScreenAsync()
        {
            _asyncBehaviour.StartCoroutine(DetectScreen());
        }

        public IEnumerator DetectScreen()
        {
            //Wait for graphics to render
            yield return new WaitForEndOfFrame();

            //Create a texture to pass to encoding
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            //Put buffer into texture
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

            yield return Detect(texture);
        }

        public void DetectAsync(Texture2D texture)
        {
            _asyncBehaviour.StartCoroutine(Detect(texture));
        
        }

        public IEnumerator Detect(Texture2D texture)
        {
            // Get the PNG
            var data = texture.EncodeToPNG();

            // No query customisations yet, but in the future the query string can be 
            // formatted here
            var queryString = String.Format(FaceStrings.QUERY_STRING_FORMAT_DETECT,
                                            true, true, FaceStrings.PARAMETER_STRING_FACE_ATTRIBUTES);
            var requestUrl = FaceStrings.URL_DETECT + queryString;
            if (Verbose) Debug.LogFormat("Detect URL {0}.", requestUrl);

            // Add the headers
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(CognitiveStrings.HEADER_OCP_APIM_SUBSCRIPTION_KEY, APIKey);
            headers.Add(CognitiveStrings.HEADER_CONTENT_TYPE, CognitiveStrings.CONTENT_TYPE_OCTET_STREAM);

            // The fantastic Unity WWW request
            var client = new WWW(requestUrl, data, headers);

            yield return client;

            // Dump the response for now
            if (Verbose) Debug.LogFormat("Detect Response {0}.", client.text);

            var facesDetected = JsonConvert.DeserializeObject<List<DetectResponse.RootObject>>(client.text);

            var args = new FaceAPIEventArgs
            {
                FacesDetected = facesDetected,
                OriginalImage = texture
            };

            if (OnDetect != null)
            {
                OnDetect(this, args);
            }
        }

    }
}
