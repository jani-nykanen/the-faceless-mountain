using System;
using System.Xml.Linq;
using System.Collections.Generic;


namespace monogame_experiment.Desktop.Core
{
	// Key configuration class
    public class KeyConfig
    {
      
		// Keys
		private List<KeyValuePair<int>> keys;


		// Constructor
        public KeyConfig(String path)
        {
        
			keys = new List<KeyValuePair<int>>();
                     
            // Open document
            XElement doc = XElement.Load(path);

            // Go through nodes
            String key, value;
            foreach (XElement e in doc.Elements())
            {
                // Get key & value
                key = e.Attribute("name").Value;
                value = e.Attribute("code").Value;

				// Add a parameter
				keys.Add(new KeyValuePair<int>(key, int.Parse(value)));
            }

        }


        // Get a key index
        public int GetKeyIndex(String name) 
		{

			foreach(KeyValuePair<int> kv in keys)
			{
				if(kv.key.Equals(name))
				{
					return kv.value;
				}
			}
			return 0;
		}
    }
}
