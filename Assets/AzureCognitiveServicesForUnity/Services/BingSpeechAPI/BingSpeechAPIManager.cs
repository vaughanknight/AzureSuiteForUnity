using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BingSpeechAPIManager : CognitiveServicesBehaviour<BingSpeechAPIManager>
{
    private BingSpeechAPI _bingSpeechAPI { get; set; }
    void Start()
    {
        _bingSpeechAPI = new BingSpeechAPI(CognitiveKeys.BingSpeechAPI, this);

        _bingSpeechAPI.OnAcquireToken += _bingSpeechAPI_OnAcquireToken;
        
        // At launch get the auth token
        _bingSpeechAPI.AcquireTokenAsync();
    }

    private void _bingSpeechAPI_OnAcquireToken(BingSpeechAPI sender, BingSpeechAPI.AcquireTokenEventArgs args)
    {
        Debug.Log(args.Token);
    }

    private void _bingSpeechAPI_OnRecognise(BingSpeechAPI sender, BingSpeechAPI.RecogniseEventArgs args)
    {
        Debug.Log(args.JsonResponse);
    }

    public AudioClip _audioToRecognise;
    private int _RATE = 16000;
    private int _SECONDS = 3;
    private AudioClip _recordingClip;
    
    IEnumerator RecordSeconds(int seconds)
    {
        Debug.Log("Recording... ");
        _recordingClip = Microphone.Start(null, false, seconds, _RATE);

        Debug.Log("Recording... STARTED");
        yield return new WaitForSeconds(seconds);
        Debug.Log("Recording... DONE");
        
        // Write the audio to disk
        Debug.Log("Saving...");
        //var wavData = new WavData(_recordingClip);
        //wavData.Save("c:\\projects\\vaughan\\test.wav");

        _bingSpeechAPI.RecogniseAsync(_recordingClip);
        Debug.Log("Saving... DONE");
    }
}
