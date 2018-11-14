using System;
namespace monogame_experiment.Desktop.Core
{
    // Audio manager
    public class AudioManager
    {
        private bool enabled;

        // Constructor
        public AudioManager()
        {
            enabled = true;
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
    }
}
