using Godot;
using System;

public class OptionsMenu : PrimeScene
{
    private CheckButton FullScreenButton = null;
    private Slider MusicSlider = null;
    private Slider SFXSlider = null;

    #region Initialization

        public override void _EnterTree()
        {
            PauseMode = PauseModeEnum.Process;
        }

        public override void _Ready()
        {
            base._Ready();
            FullScreenButton = GetNode<CheckButton>("Center2/Menu/FullScreenButton");
            if (FullScreenButton != null)
            {
                FullScreenButton.Pressed = OS.WindowFullscreen;
                FullScreenButton.Connect("pressed", this, nameof(OnFullScreenButtonPressed));
            }
            MusicSlider = GetNode<Slider>("Center2/Menu/MusicSlider");
            if (MusicSlider != null)
            {
                MusicSlider.MinValue = Audio.MinVolumeDb;
                MusicSlider.MaxValue = Audio.MaxVolumeDb;
	            MusicSlider.Value = Audio.GetMusicVolume();
                MusicSlider.Connect("value_changed", this, nameof(OnMusicSliderChanged));
            }
            SFXSlider = GetNode<Slider>("Center2/Menu/SFXSlider");
            if (SFXSlider != null)
            {
                SFXSlider.MinValue = Audio.MinVolumeDb;
                SFXSlider.MaxValue = Audio.MaxVolumeDb;
	            SFXSlider.Value = Audio.GetSFXVolume();
                SFXSlider.Connect("value_changed", this, nameof(OnSFXSliderChanged));
            }
        }

    #endregion

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (ev.IsActionPressed(InputActions.UI_CANCEL))
            {
                Scene.PopSub();
            }
        }

    #endregion

    #region UI Callbacks
        
        private void OnBackButtonPressed()
        {
            Scene.PopSub();
        }

        private void OnFullScreenButtonPressed()
        {
            Prime.ToggleFullScreen();
        }

        private void OnMusicSliderChanged(float value)
        {
            Audio.SetBusVolume(Audio.MUSIC_BUS_NAME, value);
            GetNode<AudioStreamPlayer>("MusicDing").Play();
        }

        private void OnSFXSliderChanged(float value)
        {
            Audio.SetBusVolume(Audio.SFX_BUS_NAME, value);
            GetNode<AudioStreamPlayer>("SFXDing").Play();
        }

    #endregion
}
