using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;




public class BingSpeechAPIManager : CognitiveServicesBehaviour<BingSpeechAPIManager>, ICognitiveServicesBehaviour
{
    private BingSpeechAPI _bingSpeechAPI { get; set; }
    public AudioClip audioGenerated;
    
    public void Start()
    {
        _bingSpeechAPI = new BingSpeechAPI("94c91aeef0af42ef944e988f520514fd", this);

        _bingSpeechAPI.TextToSpeechAsync("Hello There");
        _bingSpeechAPI.OnRecognise += _bingSpeechAPI_OnRecognise;
        _bingSpeechAPI.RecogniseAsync(_audioToRecognise);
    }

    private void _bingSpeechAPI_OnRecognise(BingSpeechAPI sender, BingSpeechAPI.RecogniseEventArgs args)
    {
        Debug.Log(args.JsonResponse);
    }

    private void _bingSpeechAPI_OnTextToSpeech(BingSpeechAPI sender, BingSpeechAPI.TextToSpeechEventArgs args)
    {
        Debug.Log("Got response");
        audioGenerated = args.GeneratedAudio;
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
