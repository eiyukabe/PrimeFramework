using Godot;

/// <summary> Static class for audio-related functionality.static </summary>
public static class Audio
{
    public const string MUSIC_BUS_NAME = "Music"; 
    public const string SFX_BUS_NAME = "SFX"; 

    public static int MinVolumeDb = -60; /// <summary> Min volume (in decibals) an audio slider can reach.static </summary>
    public static int MaxVolumeDb = -0; /// <summary> Max volume (in decibals) an audio slider can reach.static </summary>

    public static float GetMusicVolume()
    {
        return AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(MUSIC_BUS_NAME));
    }

    public static float GetSFXVolume()
    {
        return AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(SFX_BUS_NAME));
    }

    public static void SetBusVolume(string busName, float volume)
    {
        int BusIndex = AudioServer.GetBusIndex(busName);
        AudioServer.SetBusVolumeDb(BusIndex, volume);
        AudioServer.SetBusMute(BusIndex, volume == MinVolumeDb);
    }
}