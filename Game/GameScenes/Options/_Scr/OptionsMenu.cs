using Godot;
using System;

public class OptionsMenu : GameScene
{
    private Slider MusicSlider = null;
    private Slider SFXSlider = null;

    #region Initialization

        public override void _Ready()
        {
            base._Ready();
            MusicSlider = GetNode<Slider>("canvas_layer/music_slider");
            if (MusicSlider != null)
            {
                MusicSlider.MinValue = Audio.MinVolumeDb;
                MusicSlider.MinValue = Audio.MaxVolumeDb;
	            MusicSlider.SetValue(Audio.GetMusicVolume());
                MusicSlider.Connect("value_changed", this, nameof(OnMusicSliderChanged));
            }
            SFXSlider = GetNode<Slider>("canvas_layer/sound_slider");
            if (SFXSlider != null)
            {
                SFXSlider.MinValue = Audio.MinVolumeDb;
                SFXSlider.MinValue = Audio.MaxVolumeDb;
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

        private void OnMusicSliderChanged(float value)
        {
            Audio.SetBusVolume(Audio.MUSIC_BUS_NAME, value);
        }

        private void OnSFXSliderChanged(float value)
        {
            Audio.SetBusVolume(Audio.SFX_BUS_NAME, value);
        }

    #endregion
}
