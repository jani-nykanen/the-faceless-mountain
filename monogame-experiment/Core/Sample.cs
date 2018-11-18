using System;
using System.IO;

using Microsoft.Xna.Framework.Audio;


namespace monogame_experiment.Desktop.Core
{
    // Not really a sample, but close enough
    public class Sample
    {
        // Sound
        private SoundEffect sound;
        // Sound effect instance
        private SoundEffectInstance instance;


        // Load from a file
        public Sample(String path)
        {
            // Open the file
            FileStream fs = new FileStream(path, FileMode.Open);

            // Create a sound
            sound = SoundEffect.FromStream(fs);
            // Create sound effect instance
            instance = sound.CreateInstance();

            // Dispose file stream
            fs.Dispose();
        }


        // Play sound
        public void Play(float vol, bool loop = false)
        {
            // Stop, if already playing
            if(instance.State == SoundState.Playing ||
               instance.State == SoundState.Paused)
                instance.Stop();

            instance.Volume = vol;
            instance.IsLooped = loop;
            instance.Play();
        }


        // Get volume
        public float GetVolume()
        {
            return instance.Volume;
        }


        // Set volume
        public void SetVolume(float v)
        {
            if (v < 0.0f) v = 0.0f;
            if (v >= 1.0f) v = 1.0f;

            instance.Volume = v;
        }


        // Pause
        public void Pause()
        {
            instance.Pause();
        }


        // Resume
        public void Resume()
        {
            instance.Resume();
        }


        // Stop
        public void Stop()
        {
            instance.Stop();
        }


        // Destroy
        public void Destroy()
        {
            instance.Dispose();
            sound.Dispose();
        }
    }
}
