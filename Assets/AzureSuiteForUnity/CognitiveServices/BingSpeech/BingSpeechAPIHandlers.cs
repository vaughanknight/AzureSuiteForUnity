using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureSuiteForUnity.CognitiveServices.BingSpeech
{
    public class BingSpeechAPIHandlers
    {
        public delegate void RecogniseEventHandler(IBingSpeechAPI sender, RecogniseEventArgs args);
        public delegate void TextToSpeechEventHandler(IBingSpeechAPI sender, TextToSpeechEventArgs args);
    }
}
