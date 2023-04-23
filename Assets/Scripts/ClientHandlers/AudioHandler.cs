using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Opus;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler Instance;
    public List<AudioStreamFile> streamingAssets = new List<AudioStreamFile>();

    public static void OnLoad()
    {
        if (Instance != null)
        {
            return;
        }

        GameObject go = new GameObject("Audio Handler");
        AudioHandler newManager = go.AddComponent<AudioHandler>();
        newManager.Initialize();
        Instance = newManager;
    }

    public void Initialize()
    {
        Bass.BASS_Free();
        Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
        Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 500);
        BASSError error = Bass.BASS_ErrorGetCode();

        if (error != BASSError.BASS_OK && error != BASSError.BASS_ERROR_INIT)
            Debug.LogError($"BASS ERROR: {error}");        
    }
    public void CreateAudioStream(string location, bool isLoop = false)
    {
        Stream stream = AppHandler.Instance.SearchMPQ(location);
        long length = stream.Length;
        byte[] buffer = new byte[length];
        stream.Read(buffer, 0, (int)length);

        AudioStreamFile audioStreamFile = new AudioStreamFile()
        {
            Name = location,
            buffer = buffer,
            loop = isLoop
        };
        CreateStream(audioStreamFile);
    }
    public void CreateStream(AudioStreamFile inputFile)
    {        
        int bassFxVersion = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_GetVersion();

        BASSFlag CreateFlags = BASSFlag.BASS_SAMPLE_FX | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_ASYNCFILE | BASSFlag.BASS_STREAM_PRESCAN;

        var _hGCFile = GCHandle.Alloc(inputFile.buffer, GCHandleType.Pinned);

        inputFile.streamHandle = Bass.BASS_StreamCreateFile(_hGCFile.AddrOfPinnedObject(), 0L, inputFile.buffer.Length, CreateFlags);

        if (inputFile.streamHandle != 0)
            inputFile.streamHandle = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(inputFile.streamHandle, BASSFlag.BASS_FX_FREESOURCE);

        CheckError();
        if (inputFile.streamHandle != 0)
        {
            streamingAssets.Add(inputFile);
            Bass.BASS_ChannelPlay(inputFile.streamHandle, false);
        }
    }

    void OnDisable() { Destroy(); }
    private void OnApplicationQuit() { Destroy(); }
    private void OnDestroy() { Destroy(); }
    public void Destroy()
    {
        for(int i = 0; i < streamingAssets.Count; i++)
            DestroyStream(streamingAssets[i]);

        Bass.BASS_Free();
        CheckError();
    }
    public void DestroyStream(AudioStreamFile file)
    {
        Bass.BASS_StreamFree(file.streamHandle);
        CheckError();
        streamingAssets.Remove(file);
    }
    void CheckError()
    {
        BASSError error = Bass.BASS_ErrorGetCode();
        if (error != BASSError.BASS_OK)
            Debug.LogError($"BASSError: {error}");        
    }
    public double GetPlaybackPos(int handle)
    {
        double max = -1;
        long bytes = Bass.BASS_ChannelGetPosition(handle);
        double pos = Bass.BASS_ChannelBytes2Seconds(handle, bytes);
        return Math.Max(pos, max);;
    }
    public double GetLength(int handle)
    {
        double max = -1;
        long bytes = Bass.BASS_ChannelGetLength(handle, BASSMode.BASS_POS_BYTE);
        double seconds = Bass.BASS_ChannelBytes2Seconds(handle, bytes);
        return Math.Max(max, seconds); ;
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < streamingAssets.Count; i++)
        {
            if (GetPlaybackPos(streamingAssets[i].streamHandle) >= GetLength(streamingAssets[i].streamHandle))
            {
                if (streamingAssets[i].loop)
                {
                    Bass.BASS_ChannelPlay(streamingAssets[i].streamHandle, true);
                }
                else 
                {
                    DestroyStream(streamingAssets[i]);
                }
            }
        }
    }
}
public class AudioStreamFile
{
    public string Name;
    public int streamHandle;
    public byte[] buffer;
    public bool loop = false;
}
