﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CognitiveServicesBehaviour<T> : 
    SingletonBehaviour<T> where T : MonoBehaviour, ICognitiveServicesBehaviour
{
    public string APIKey { get; set; }
    public string Token { get; set; }
    public string Title { get { return "Default Title"; }  }

}

public interface ICognitiveServicesBehaviour
{
    string APIKey { get; set; }
    string Token { get; set; }
    string Title { get; }
}

