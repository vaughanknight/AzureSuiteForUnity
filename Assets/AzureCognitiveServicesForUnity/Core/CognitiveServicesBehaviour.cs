using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CognitiveServicesBehaviour<T> : SingletonBehaviour<T> where T : MonoBehaviour
{
    protected CognitiveServicesKeyList CognitiveKeys
    {
        get
        {
            return CognitiveServicesKeyManager.Instance.Keys;
        }
    }
}
