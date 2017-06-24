using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.BingSpeech
{
    public class TextToSpeechEventArgs
    {
        public TextToSpeechEventArgs(bool success, AudioClip generatedAudio)
        {
            Success = success;
            GeneratedAudio = generatedAudio;
        }

        public bool Success { get; private set; }
        public AudioClip GeneratedAudio { get; private set; }
    }
}
