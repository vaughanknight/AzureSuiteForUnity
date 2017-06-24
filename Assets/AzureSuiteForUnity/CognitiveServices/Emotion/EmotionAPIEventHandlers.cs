using AzureSuiteForUnity.CognitiveServices.Emotion.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Emotion
{
    public class EmotionAPIEventArgs
    {
        public IList<RecognizeResponse.Emotion> EmotionsDetected { get; internal set; }
        public Texture2D OriginalImage { get; internal set; }
    }
    public class EmotionAPIEventHandlers
    {
        public delegate void RecognizeEventHandler(object sender, EmotionAPIEventArgs args);
    }
}
