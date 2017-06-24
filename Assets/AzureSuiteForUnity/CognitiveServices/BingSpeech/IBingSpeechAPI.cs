using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.BingSpeech
{
    public interface IBingSpeechAPI
    {
        event BingSpeechAPIHandlers.RecogniseEventHandler OnRecognise;
        event BingSpeechAPIHandlers.TextToSpeechEventHandler OnTextToSpeech;

        void RecogniseAsync(AudioClip clip);
        void TextToSpeechAsync(string text);
        
        string APIKey { get; set; }
    }
}
