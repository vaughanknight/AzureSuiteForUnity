using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AzureSuiteForUnity.CognitiveServices
{
    /// <summary>
    /// A Token Manager that allows for threadsafe token acquisition.  This is to avoid having
    /// 50 tokens across 50 objects in the game that request in parallel.
    /// </summary>
    public class TokenManager : SingletonBehaviour<TokenManager>
    {
        private Dictionary<string, string> _tokenCache = new Dictionary<string, string>();

        private class TokenManagerStrings
        {
            //public const string HEADER_OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
            public const string URI_COGNITIVE_TOKEN = "https://api.cognitive.microsoft.com/sts/v1.0/issueToken";
        }

        public delegate void AcquireTokenEventHandler(TokenManager sender, AcquireTokenEventArgs args);
        public class AcquireTokenEventArgs
        {
            public AcquireTokenEventArgs(string token)
            {
                Token = token;
            }
            public string Token { get; private set; }
        }

        private bool _mutex = false;

        public void AcquireTokenAsync(string APIKey, AcquireTokenEventHandler handler)
        {
            StartCoroutine(AcquireToken(APIKey, handler));
        }

        public IEnumerator WaitForMutex(string APIKey, AcquireTokenEventHandler handler)
        {
            yield return new WaitUntil(() => { return !_mutex; });
            var args = new AcquireTokenEventArgs(_tokenCache[APIKey]);
        }

        public IEnumerator AcquireToken(string APIKey, AcquireTokenEventHandler handler)
        {
            if (_mutex)
            {
                yield return new WaitUntil(() => { return !_mutex; });
            }
            _mutex = true;

            if (_tokenCache.ContainsKey(APIKey))
            {
                var cachedToken = _tokenCache[APIKey];
                var args = new AcquireTokenEventArgs(cachedToken);
                handler.Invoke(this, args);
            }
            else
            {
                // Create headers
                var headers = new Dictionary<string, string>();

                //headers.Add(Strings.HEADER_CONTENT_TYPE, Strings.CONTENT_TYPE_APPLICATION_OCTET_STREAM);
                headers.Add(CognitiveStrings.HEADER_OCP_APIM_SUBSCRIPTION_KEY, APIKey);

                // Request dummy data
                var dummyData = new byte[1] { 1 };

                // If the endpoint gets 0 bytes it returns an error,
                // so we send 1 byte of dummy data
                var w = new WWW(TokenManagerStrings.URI_COGNITIVE_TOKEN, dummyData, headers);
                yield return w;

                // Response
                if (string.IsNullOrEmpty(w.error))
                {
                    var token = w.text;

                    _tokenCache.Add(APIKey, token);

                    var args = new AcquireTokenEventArgs(token);
                    if (handler != null)
                    {
                        handler.Invoke(this, args);
                    }
                }
                else
                {
                    Debug.LogError("Error obtaining auth token: " + w.error);
                }
                _mutex = false;
            }
        }
    }

}