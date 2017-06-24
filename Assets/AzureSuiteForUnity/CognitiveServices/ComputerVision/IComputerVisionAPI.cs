using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AzureSuiteForUnity.CognitiveServices.ComputerVision
{
    public interface IComputerVisionAPI
    {
        IEnumerator DescribeScreen();
        IEnumerator Describe(Texture2D texture);
        void DescribeScreenAsync();
        void DescribeAsync(Texture2D texture);
        bool Verbose { get; set; }
        string APIKey { get; set; }

        event ComputerVisionAPIEventHandlers.DescribeEventHandler OnDescribe;
    }
}
