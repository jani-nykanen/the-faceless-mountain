using System;
namespace monogame_experiment.Desktop.Core
{
	// Key value pair
    public struct KeyValuePair<T>
    {
		public String key;
        public T value;
        
        // Constructor
		public KeyValuePair(String key, T value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
