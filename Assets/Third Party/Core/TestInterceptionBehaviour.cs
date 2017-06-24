using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization;
using AzureSuiteForUnity.CognitiveServices;
using Castle.Core.Interceptor;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.DynamicProxy;
using Component = Castle.MicroKernel.Registration.Component;

[Serializable]
public class LoggingInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        if(invocation.Method.Name.ToLower().Contains("detect"))
        { 
            Debug.LogFormat("{0}::{1}", invocation.TargetType.Name, invocation.Method.Name);
        }

        invocation.Proceed();
    }
}


[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class CognitiveTokenRequiredAttribute : Attribute
{
    public CognitiveTokenRequiredAttribute()
    {
        // Nothing yet
    }
}

public class TokenInterceptor : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        Debug.Log("Interception to get token.");
        var tokenService = (ITokenService)invocation.InvocationTarget;

        var attributes = invocation.Method.GetCustomAttributes(true);
        foreach(var attributeObject in attributes)
        {
            var attribute = (Attribute)attributeObject;
            if(attribute.GetType() == typeof(CognitiveTokenRequiredAttribute))
            {
                var token = tokenService.Token;
                if (string.IsNullOrEmpty(token))
                {
                    // Get the key here
                    Debug.Log("Do not have a token, so need to obtain one.");

                    // Get token
                    Debug.Log("This is where it would get the token.");
                    tokenService.Token = "1234";
                }
                else
                {
                    Debug.Log("Have a token:" + token);
                }
            }
        }


        try { 
            // Have a key so proceed
            invocation.Proceed();
        }
        catch(InvalidKeyException kre)
        {
            // Retry once
        }
    }
}

[Serializable]
internal class InvalidKeyException : Exception
{
    public InvalidKeyException()
    {
    }

    public InvalidKeyException(string message) : base(message)
    {
    }

    public InvalidKeyException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected InvalidKeyException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

public interface ITokenService
{
    string Token { get; set; }
}

public interface IBingSpeechAPIXXXXXX : ITokenService
{
    [CognitiveTokenRequired]
    AudioClip StringToAudio(string s);

}


//[Serializable]
//public class BingSpeechAPIService : IBingSpeechAPI
//{
//    public BingSpeechAPIService(string key)
//    {
//        Key = key;
//    }
    
//    public string Key { get; private set; }
//    public string Token { get; set; }

//    public AudioClip StringToAudio(string s)
//    {
//        Debug.Log(s);
//        return null;
//    }
//}





public class TestInterceptionBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //var service = CognitiveServicesServiceFactory.Instance.GetBingSpeechApi();

        //var s = service.StringToAudio("THIS TEXT");
        //Debug.Log("Return:" + s);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
