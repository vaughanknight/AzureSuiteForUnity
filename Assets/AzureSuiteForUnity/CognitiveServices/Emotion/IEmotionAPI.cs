using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Emotion
{
    public interface IEmotionAPI
    {
        string APIKey { get; set; }
        bool Verbose { get; set; }

        event EmotionAPIEventHandlers.RecognizeEventHandler OnRecognize;

        IEnumerator Recognize(Texture2D texture);
        IEnumerator RecognizeScreen();

        void RecognizeAsync(Texture2D texture);
        void RecognizeScreenAsync();
    }
}
