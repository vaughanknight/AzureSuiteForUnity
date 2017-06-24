using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureSuiteForUnity.CognitiveServices.BingSpeech
{
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
}
