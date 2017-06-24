using AzureSuiteForUnity.CognitiveServices.Emotion.Model;
using Castle.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Emotion
{
    [Interceptor(typeof(LoggingInterceptor))]
    public class EmotionAPI : IEmotionAPI
    {
        private struct EmotionStrings
        {
            public const string URL_RECOGNIZE = "https://westus.api.cognitive.microsoft.com/emotion/v1.0/recognize";
        }

        public event EmotionAPIEventHandlers.RecognizeEventHandler OnRecognize;

        public string APIKey { get; set; }
        public bool Verbose { get; set; }
        private MonoBehaviour _asyncBehaviour;

        public EmotionAPI(MonoBehaviour behaviour)
        {
            Verbose = false;
            _asyncBehaviour = behaviour;
            //APIKey = "d0584ebec5524eafbf5f2d8ca4fc0ae5";
        }

        public void RecognizeScreenAsync()
        {
            _asyncBehaviour.StartCoroutine(RecognizeScreen());
        }

        public IEnumerator RecognizeScreen()
        {
            //Wait for graphics to render
            yield return new WaitForEndOfFrame();

            //Create a texture to pass to encoding
            var texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

            //Put buffer into texture
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

            yield return Recognize(texture);
        }

        public void RecognizeAsync(Texture2D texture)
        {
            _asyncBehaviour.StartCoroutine(Recognize(texture));
        }

        public IEnumerator Recognize(Texture2D texture)
        {
            // Get the PNG
            var data = texture.EncodeToPNG();

            // No query in the URL
            var requestUrl = EmotionStrings.URL_RECOGNIZE;
            if (Verbose) Debug.LogFormat("Recognize URL {0}.", requestUrl);

            // Add the headers
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add(CognitiveStrings.HEADER_OCP_APIM_SUBSCRIPTION_KEY, APIKey);
            headers.Add(CognitiveStrings.HEADER_CONTENT_TYPE, CognitiveStrings.CONTENT_TYPE_OCTET_STREAM);

            // The fantastic Unity WWW request
            var client = new WWW(requestUrl, data, headers);

            yield return client;

            if (Verbose) Debug.LogFormat("Detect Response {0}.", client.text);

            var response = JsonConvert.DeserializeObject<RecognizeResponse>(client.text);

            var args = new EmotionAPIEventArgs()
            {
                EmotionsDetected = response.Emotions,
                OriginalImage = texture
            };

            if (OnRecognize != null)
            {
                OnRecognize(this, args);
            }
        }

    }
}
