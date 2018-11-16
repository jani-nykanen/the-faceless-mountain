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

        // Fading sample
        private Sample fadingSample;
        // Fade speed
        private float fadeSpeed;
        // Fade target
        private float fadeTarget;


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


        // Fade a sample
        public void FadeSample(Sample s, int ms, float start, float target, bool loop = false)
        {
            fadingSample = s;

            // Compute speed
            fadeSpeed = (target - start) / (float)ms * (1000.0f / 60.0f);

            // Set target & start sample
            fadeTarget = volume * target;
            s.Play(start * volume, loop);
        }


        // Update
        public void Update(float tm)
        {
            // Fade in
            bool stop = false;
            if(fadingSample != null)
            {
                float v = fadingSample.GetVolume();
                v += fadeSpeed * tm;
                if( (fadeSpeed > 0.0f && v > fadeTarget)
                   || (fadeSpeed < 0.0f && v < fadeTarget)) 
                {
                    v = fadeTarget;
                    stop = true;
                }
                fadingSample.SetVolume(v);

                // Stop, target reached
                if (stop)
                    fadingSample = null;
            }
        }
    }
}
