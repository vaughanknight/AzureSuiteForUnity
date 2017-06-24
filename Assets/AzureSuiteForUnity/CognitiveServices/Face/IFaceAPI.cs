using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Face
{
    public interface IFaceAPI
    {
        string APIKey { get; set; }
        bool Verbose { get; set; }

        event FaceAPIEventHandlers.DetectEventHandler OnDetect;

        IEnumerator Detect(Texture2D texture);
        IEnumerator DetectScreen();

        void DetectAsync(Texture2D texture);
        void DetectScreenAsync();
    }
}
