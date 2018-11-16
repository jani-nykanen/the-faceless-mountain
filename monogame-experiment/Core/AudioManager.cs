using System;
namespace monogame_experiment.Desktop.Core
{
    // Audio manager
    public class AudioManager
    {
        // Is audio enabled
        private bool enabled;

        // Audio volume
        private float volume;


        // Constructor
        public AudioManager()
        {
            enabled = true;
            volume = 1.0f;
        }


        // Toggle
        public void ToggleAudio()
        {
            enabled = !enabled;
        }


        // Is enabled
        public bool IsEnabled()
        {
            return enabled;
        }


        // Set music volume
        public void SetAudioVol(float vol)
        {
            volume = vol;
        }


        // Play a sample
        public void PlaySample(Sample s, float vol, bool loop = false)
        {
            s.Play(vol * volume, loop);
        }
    }
}
