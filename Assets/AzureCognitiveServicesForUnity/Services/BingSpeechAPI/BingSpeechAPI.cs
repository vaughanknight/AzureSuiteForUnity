using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for BingSearchAPI.  
/// 
/// NOTE: Leverages asynchronous methods with callback event handlers 
/// and not async await due to Unity backwards compatibility.  This 
/// also cleans up nested StartCoroutine() and IEnumerator yield return spaghetti.
/// </summary>
public class BingSpeechAPI
{
    private struct Strings
    {
        public const string HEADER_CONTENT_TYPE = "Content-Type";
        public const string HEADER_OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
        public const string HEADER_AUTHORIZATION = "Authorization";

        public const string CONTENT_TYPE_AUDIO_WAV_FORMAT = "audio/wav; codec=audio/pcm; samplerate={0};";
        public const string CONTENT_TYPE_APPLICATION_OCTET_STREAM = "application/octet-stream";
        public const string AUTHORIZATION_BEARER_FORMAT = "Bearer {0}";

        public const string PARAMETER_APP_ID = "D4D52672-91D7-4C74-8AD8-42B1D98141A5";

        public const string QUERY_STRING_SPEECH_API_FORMAT = "?Version=3.0&requestid={0}&appID={1}&format=json&locale=en-US&device.os=Unity&scenarios=ulm&instanceid={2}";

        public const string URI_SPEECH_RECOGNISE = "https://speech.platform.bing.com/recognize";
        public const string URI_COGNITIVE_TOKEN = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
    }


    public delegate void AcquireTokenEventHandler(BingSpeechAPI sender, AcquireTokenEventArgs args);
    public event AcquireTokenEventHandler OnAcquireToken;
    public class AcquireTokenEventArgs
    {
        public AcquireTokenEventArgs(bool success, string token)
        {
            Success = success;
            Token = token;
        }
        public bool Success { get; private set; }
        public string Token { get; private set; }
    }


    public delegate void RecogniseEventHandler(BingSpeechAPI sender, RecogniseEventArgs args);
    public event RecogniseEventHandler OnRecognise;
    public class RecogniseEventArgs
    {
        public RecogniseEventArgs(bool success, string jsonResponse)
        {
            Success = success;
            JsonResponse = jsonResponse;
        }

        public bool Success { get; private set; }
        public string JsonResponse { get; private set; }
    }
  

    private string _token { get; set; }
    private string _key { get; set; }

    /// <summary>
    /// Behaviour is for coroutines, to ensure everything runs nicely in
    /// Unity threading space, and callbacks don't happen on non game loop threads
    /// </summary>
    private MonoBehaviour _behaviour { get;set;}

    public BingSpeechAPI(string key, MonoBehaviour behaviour)
    {
        _behaviour = behaviour;
        _key = key;
    }

    public BingSpeechAPI(string key, string token, MonoBehaviour behaviour)
    {
        _behaviour = behaviour;
        _key = key;
    }

    public void AcquireTokenAsync()
    {
        _behaviour.StartCoroutine(GetToken());
    }

    private IEnumerator GetToken()
    {
        // Create headers
        var headers = new Dictionary<string, string>();
        headers.Add(Strings.HEADER_CONTENT_TYPE, Strings.CONTENT_TYPE_APPLICATION_OCTET_STREAM);
        headers.Add(Strings.HEADER_OCP_APIM_SUBSCRIPTION_KEY, _key);

        // Request dummy data
        var dummyData = new byte[1] { 1 };

        // If the endpoint gets 0 bytes it returns an error,
        // so we send 1 byte of dummy data
        var w = new WWW(Strings.URI_COGNITIVE_TOKEN, dummyData, headers);
        yield return w;

        // Response
        if (w.error == null)
        {
            _token = w.text;
            var args = new AcquireTokenEventArgs(true, _token);
            if(OnAcquireToken != null)
            {
                OnAcquireToken(this, args);
            }
        }
        else
        {
            Debug.LogError("Error obtaining auth token: " + w.text);
        }
    }

    public void RecogniseAsync(AudioClip clip)
    {
        _behaviour.StartCoroutine(Recognise(clip));
    }

    private IEnumerator Recognise(AudioClip clip)
    {
        var wavData = new WavData(clip);

        var queryString = String.Format(Strings.QUERY_STRING_SPEECH_API_FORMAT, Guid.NewGuid(), Strings.PARAMETER_APP_ID, Guid.NewGuid());
        var speechRecognitionUri = Strings.URI_SPEECH_RECOGNISE + queryString;

        var headers = CreateRecogniseHeaders(clip);

        // Request
        var w = new WWW(speechRecognitionUri, wavData.FullRawBytes, headers);
        yield return w;

        // Response
        if (w.error == null)
        {
            if(OnRecognise != null)
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
        var contentTypeHeader = String.Format(Strings.CONTENT_TYPE_AUDIO_WAV_FORMAT, clip.frequency);
        headers.Add(Strings.HEADER_CONTENT_TYPE, contentTypeHeader);
        var bearerHeader = String.Format(Strings.AUTHORIZATION_BEARER_FORMAT, _token);
        headers.Add(Strings.HEADER_AUTHORIZATION, bearerHeader);
        return headers;
    }
}
