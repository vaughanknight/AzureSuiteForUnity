//	This software is provided 'as-is', without any express or implied warranty. In
//	no event will the authors be held liable for any damages arising from the use
//	of this software.
//
//	Permission is granted to anyone to use this software for any purpose,
//	including commercial applications, and to alter it and redistribute it freely,
//	subject to the following restrictions:
//
//	1. The origin of this software must not be misrepresented; you must not claim
//	that you wrote the original software. If you use this software in a product,
//	an acknowledgment in the product documentation would be appreciated but is not
//	required.
//
//	2. Altered source versions must be plainly marked as such, and must not be
//	misrepresented as being the original software.
//
//	3. This notice may not be removed or altered from any source distribution.
//
//  =============================================================================
//
//  derived from Calvin Rien's SavWav http://the.darktable.com script
//  https://gist.github.com/darktable/2317063
//  which was derived from Gregorio Zanon's script
//  http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734

using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class for passing around AudioClips and generated 
/// wav data for transmitting to services or writing to file
/// </summary>
public class WavData
{
    const int HEADER_SIZE = 44;

    private AudioClip _clip { get; set; }
    public byte[] FullRawBytes { get; private set; }

    public WavData(AudioClip clip)
    {
        _clip = clip;
        FullRawBytes = GetFullBytes();
    }

    /// <summary>
    /// Save he audio to a file
    /// </summary>
    /// <param name="filename">The name of the file to save.</param>
    /// <returns></returns>
    public bool Save(string filename)
    {
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        var filepath = Path.Combine(Application.persistentDataPath, filename);

        Debug.Log(filepath);

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));

        using (var fileStream = CreateEmpty(filepath))
        {
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(FullRawBytes, 0, FullRawBytes.Length);
            fileStream.Flush();
            fileStream.Close();
        }

        return true; // TODO: return false if there's a failure saving the file
    }

    public AudioClip TrimSilence(float min)
    {
        var samples = new float[_clip.samples];

        _clip.GetData(samples, 0);

        return TrimSilence(new List<float>(samples), min, _clip.channels, _clip.frequency);
    }

    public AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
    {
        return TrimSilence(samples, min, channels, hz, false);
    }

    public AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool stream)
    {
        int i;

        for (i = 0; i < samples.Count; i++)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(0, i);

        for (i = samples.Count - 1; i > 0; i--)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(i, samples.Count - i);

        var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);

        clip.SetData(samples.ToArray(), 0);

        return clip;
    }

    /// <summary>
    /// Create an empty file.
    /// </summary>
    /// <param name="filepath">Path of the file to save.</param>
    /// <returns></returns>
    private FileStream CreateEmpty(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }

    /// <summary>
    /// Gets the full bytes of the file.
    /// </summary>
    /// <returns>The byte array of the full file.</returns>
    private byte[] GetFullBytes()
    {
        // Grab the bytes of the audio clip
        var bytes = GetRawAudioBytes();

        // Create the buffer for the audio data and the header
        // Header is always 44 bytes
        var fullBuffer = new byte[bytes.Length + HEADER_SIZE];

        // Now let's write the audio stream
        var memoryStream = new MemoryStream(fullBuffer);
        // And insert the header
        // WriteHeader(memoryStream);

        memoryStream.Seek(HEADER_SIZE, SeekOrigin.Begin);
        memoryStream.Write(bytes, 0, bytes.Length);
        memoryStream.Seek(0, SeekOrigin.Begin);

        // And insert the header
        WriteHeader(memoryStream);

        // Make sure it's written to the buffer 
        memoryStream.Flush();

        return fullBuffer;
    }

    /// <summary>
    /// Gets the wav raw bytes for the audio data only, without wav headers
    /// </summary>
    /// <returns>WAV audio data raw bytes, without headers</returns>
    private byte[] GetRawAudioBytes()
    {
        var samples = new float[_clip.samples];
        _clip.GetData(samples, 0);

        // Convert in 2 steps to Int16[], 
        Int16[] intData = new Int16[samples.Length];

        // then Int16[] to Byte[].  16 bits being 2 bytes so 2x array size
        Byte[] bytesData = new Byte[samples.Length * 2];

        // To convert float to Int16
        float rescaleFactor = 32767; 

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        return bytesData;
    }

    /// <summary>
    /// Writes the WAV file header to the first 44 bytes of the stream
    /// </summary>
    /// <param name="stream">The stream that has the audio data in it.</param>
    private void WriteHeader(Stream stream)
    {
        var hz = _clip.frequency;
        var channels = _clip.channels;
        var samples = _clip.samples;

        stream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        stream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(stream.Length - 8);
        stream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        stream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        stream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        stream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        stream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        stream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        stream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
        stream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        stream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        stream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        stream.Write(subChunk2, 0, 4);
    }
}