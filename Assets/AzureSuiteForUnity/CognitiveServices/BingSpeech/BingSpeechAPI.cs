using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.BingSpeech
{
    /// <summary>
    /// Interface for BingSearchAPI.  
    /// 
    /// NOTE: Leverages asynchronous methods with callback event handlers 
    /// and not async await due to Unity backwards compatibility.  This 
    /// also cleans up nested StartCoroutine() and IEnumerator yield return spaghetti.
    /// </summary>
    public class BingSpeechAPI : IBingSpeechAPI
    {
        private class BingSpeechAPIStrings : CognitiveStrings
        {
            public const string CONTENT_TYPE_AUDIO_WAV_FORMAT = "audio/wav; codec=audio/pcm; samplerate={0};";
            public const string CONTENT_TYPE_APPLICATION_OCTET_STREAM = "application/octet-stream";
            public const string AUTHORIZATION_BEARER_FORMAT = "Bearer {0}";

            public const string PARAMETER_APP_ID = "D4D52672-91D7-4C74-8AD8-42B1D98141A5";

            public const string QUERY_STRING_SPEECH_API_FORMAT = "?Version=3.0&requestid={0}&appID={1}&format=json&locale=en-US&device.os=Unity&scenarios=ulm&instanceid={2}";

            public const string URI_SPEECH_RECOGNISE = "https://speech.platform.bing.com/recognize";
            public const string URI_COGNITIVE_TOKEN = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
            public const string URI_TEXT_TO_SPEECH = "https://speech.platform.bing.com/synthesize";
        }

        public event BingSpeechAPIHandlers.RecogniseEventHandler OnRecognise;
        public event BingSpeechAPIHandlers.TextToSpeechEventHandler OnTextToSpeech;
        
        private string _token { get; set; }
        public string APIKey { get; set; }

        /// <summary>
        /// Behaviour is for coroutines, to ensure everything runs nicely in
        /// Unity threading space, and callbacks don't happen on non game loop threads.
        /// When injected via the service factory it is the ServiceFactory.
        /// </summary>
        private MonoBehaviour _asyncBehaviour { get; set; }

        public BingSpeechAPI(MonoBehaviour monoBehaviour)
        {
            Debug.Log("Monobehaviour is:" + monoBehaviour);
            _asyncBehaviour = monoBehaviour;
        }

        private void OnTokenAcquired(TokenManager sender, TokenManager.AcquireTokenEventArgs args)
        {
            this._token = args.Token;
        }

        public BingSpeechAPI(string key, string token)
        {
            APIKey = key;
            _token = token;
        }
        
        public void RecogniseAsync(AudioClip clip)
        {
            if (_token == null)
            {
                TokenManager.Instance.AcquireTokenAsync(APIKey, (tts, args) =>
                {
                    _token = args.Token;
                    _asyncBehaviour.StartCoroutine(Recognise(clip));
                });
            }
            else
            {
                _asyncBehaviour.StartCoroutine(Recognise(clip));
            }
        }

        private IEnumerator Recognise(AudioClip clip)
        {
            var wavData = new WavData(clip);

            var queryString = String.Format(BingSpeechAPIStrings.QUERY_STRING_SPEECH_API_FORMAT, Guid.NewGuid(), BingSpeechAPIStrings.PARAMETER_APP_ID, Guid.NewGuid());
            var speechRecognitionUri = BingSpeechAPIStrings.URI_SPEECH_RECOGNISE + queryString;

            var headers = CreateRecogniseHeaders(clip);

            // Request
            var w = new WWW(speechRecognitionUri, wavData.FullRawBytes, headers);
            yield return w;

            // Response
            if (string.IsNullOrEmpty(w.error))
            {
                if (OnRecognise != null)
                {
                    var args = new RecogniseEventArgs(true, w.text);
                    OnRecognise(this, args);
                }
            }
            else
            {
                Debug.Log("Error: " + w.text);
            }
        }

        private Dictionary<string, string> CreateRecogniseHeaders(AudioClip clip)
        {
            var headers = new Dictionary<string, string>();
            var contentTypeHeader = String.Format(BingSpeechAPIStrings.CONTENT_TYPE_AUDIO_WAV_FORMAT, clip.frequency);
            headers.Add(BingSpeechAPIStrings.HEADER_CONTENT_TYPE, contentTypeHeader);
            var bearerHeader = String.Format(BingSpeechAPIStrings.AUTHORIZATION_BEARER_FORMAT, _token);
            headers.Add(BingSpeechAPIStrings.HEADER_AUTHORIZATION, bearerHeader);
            return headers;
        }

        private Dictionary<string, string> CreateTextToSpeechHeaders()
        {
            var headers = TextToSpeechHeaders.CreateHeaders();
            return headers;
        }

        public void TextToSpeechAsync(string text)
        {
            // If we dont have a token, get one first
            if (_token == null)
            {
                TokenManager.Instance.AcquireTokenAsync(APIKey, (tts, args) =>
                {
                    _token = args.Token;
                    _asyncBehaviour.StartCoroutine(TextToSpeech(text));
                });
            }
            else
            {
                _asyncBehaviour.StartCoroutine(TextToSpeech(text));
            }
        }

        private IEnumerator TextToSpeech(string text)
        {
            var ttsParams = new TextToSpeechParameters(
                TextToSpeechParameters.VoiceType.Male,
                OutputFormatParameterValues.Raw16khz16bitMonoPCM,
                TextToSpeechParameters.Locale.en_US,
                "http://vaughanknight.com",
                _token,
                text);
            var queryString = ttsParams.ToQueryString();

            var speechRecognitionUri = BingSpeechAPIStrings.URI_TEXT_TO_SPEECH + "?" + queryString;

            var headers = CreateTextToSpeechHeaders();
            AddAuthHeader(headers);

            var dummy = new byte[1] { 1 };

            // Request
            var w = new WWW(speechRecognitionUri, dummy, headers);
            yield return w;

            // Response
            if (w.error == null)
            {
                if (OnRecognise != null)
                {
                    var args = new TextToSpeechEventArgs(true, w.GetAudioClip(false));
                    OnTextToSpeech(this, args);
                }
            }
            else
            {
                Debug.Log("Error: " + w.error);
                Debug.Log("Error: " + w.text);
            }
        }

        private void AddAuthHeader(Dictionary<string, string> headers)
        {
            var bearerHeader = String.Format(BingSpeechAPIStrings.AUTHORIZATION_BEARER_FORMAT, _token);
            headers.Add(BingSpeechAPIStrings.HEADER_AUTHORIZATION, bearerHeader);
        }

     
    }
}