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
        // Looped sample, presumably music
        private Sample loopedTrack;


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

            if (loopedTrack != null)
            {
                if (!enabled)
                    loopedTrack.Pause();
                else
                    loopedTrack.Resume();
            }

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
            // See below
            if (loop)
            {
                loopedTrack = s;
                if (!enabled)
                {
                    s.Play(vol * volume, loop);
                    s.Pause();
                }
            }

            if (!enabled) return;

            s.Play(vol * volume, loop);
        }


        // Fade a sample
        public void FadeSample(Sample s, int ms, float start, float target, bool loop = false)
        {
            // Store looped track, so we can
            // continue playing it if audio
            // is disabled and then re-enabled
            if (loop)
            {
                loopedTrack = s;
                if (!enabled)
                {
                    s.Play(target * volume, loop);
                    s.Pause();
                }
            }

            if (!enabled) return;

            fadingSample = s;

            // Compute speed
            fadeSpeed = (target - start) / (float)ms * (1000.0f / 60.0f);

            // Set target & start sample
            fadeTarget = volume * target;
            s.Play(start * volume, loop);
        }


        // Fade a sample
        public void FadeCurrentLoopedSample(int ms, float target)
        {
            fadingSample = loopedTrack;
            float start = fadingSample.GetVolume();

            // Compute speed
            fadeSpeed = (target - start) / (float)ms * (1000.0f / 60.0f);

            // Set target
            fadeTarget = volume * target;
        }


        // Update
        public void Update(float tm)
        {
            const float DELTA = 0.01f;

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
                {
                    if(v < DELTA)
                    {
                        fadingSample.Stop();
                    }
                    fadingSample = null;
                }
            }
        }
    }
}
