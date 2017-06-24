using AzureSuiteForUnity.CognitiveServices.Face.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.Face
{
    public class FaceAPIEventArgs
    {
        public List<DetectResponse.RootObject> FacesDetected { get; internal set; }
        public Texture2D OriginalImage { get; internal set; }
    }
    public class FaceAPIEventHandlers
    {
        public delegate void DetectEventHandler(object sender, FaceAPIEventArgs args);
    }
}
