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
            if(instance.State == SoundState.Playing)
                instance.Stop();

            instance.Volume = vol;
            instance.IsLooped = loop;
            instance.Play();
        }
    }
}
