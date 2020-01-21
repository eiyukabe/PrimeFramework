using Godot;
using System;

public class OptionsMenu : GameScene
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
                FullScreenButton.SetPressed(OS.IsWindowFullscreen());
                FullScreenButton.Connect("pressed", this, nameof(OnFullScreenButtonPressed));
            }
            MusicSlider = GetNode<Slider>("Center2/Menu/MusicSlider");
            if (MusicSlider != null)
            {
                MusicSlider.MinValue = Audio.MinVolumeDb;
                MusicSlider.MaxValue = Audio.MaxVolumeDb;
	            MusicSlider.SetValue(Audio.GetMusicVolume());
                MusicSlider.Connect("value_changed", this, nameof(OnMusicSliderChanged));
            }
            SFXSlider = GetNode<Slider>("Center2/Menu/SFXSlider");
            if (SFXSlider != null)
            {
                SFXSlider.MinValue = Audio.MinVolumeDb;
                SFXSlider.MaxValue = Audio.MaxVolumeDb;
	            SFXSlider.SetValue(Audio.GetSFXVolume());
                SFXSlider.Connect("value_changed", this, nameof(OnSFXSliderChanged));
            }
        }

    #endregion

    #region Input

        public override void _Input(InputEvent ev)
        {
            if (ev.IsActionPressed(InputActions.UI_CANCEL))
            {
                Prime.PopScene();
            }
        }

    #endregion

    #region UI Callbacks
        
        private void OnBackButtonPressed()
        {
            Prime.PopScene();
        }

        private void OnFullScreenButtonPressed()
        {
            Prime.ToggleFullScreen();
        }

        private void OnMusicSliderChanged(float value)
        {
            GD.Print("New Music Volume: " + value);
            Audio.SetBusVolume(Audio.MUSIC_BUS_NAME, value);
            GetNode<AudioStreamPlayer>("MusicDing").Play();
        }

        private void OnSFXSliderChanged(float value)
        {
            GD.Print("New SFX Volume: " + value);
            Audio.SetBusVolume(Audio.SFX_BUS_NAME, value);
            GetNode<AudioStreamPlayer>("SFXDing").Play();
        }

    #endregion
}
