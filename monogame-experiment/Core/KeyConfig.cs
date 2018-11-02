using System;
using System.Xml.Linq;
using System.Collections.Generic;


namespace monogame_experiment.Desktop.Core
{
	// Key configuration class
    public class KeyConfig
    {
		// Button type
		private struct Button
		{
			public int key;
			public int button;
			public String name;

            // Constructor
            public Button(String name, int key, int button)
			{
				this.key = key;
				this.button = button;
				this.name = name;
			}
		};

      
		// Keys
		private List<Button> keys;


		// Constructor
        public KeyConfig(String path)
        {
        
			keys = new List<Button>();
                     
            // Open document
            XElement doc = XElement.Load(path);

            // Go through nodes
            String name;
			int key, button;
            foreach (XElement e in doc.Elements())
            {
                // Get key & value
				name = e.Attribute("name").Value;
				key = int.Parse(e.Attribute("code").Value);
				button = -1;
                
				if(e.Attribute("button") != null)
				{
					button = int.Parse(e.Attribute("button").Value);
				}

				// Add a parameter
				keys.Add(new Button(name, key, button));
            }

        }


        // Get a key index
        public int GetKeyIndex(String name) 
		{
			foreach(Button kv in keys)
			{
				if(kv.name.Equals(name))
				{
					return kv.key;
				}
			}
			return 0;
		}


        // Get a button index
        public int GetButtonIndex(String name)
		{
			foreach (Button kv in keys)
            {
                if (kv.name.Equals(name))
                {
                    return kv.button;
                }
            }
            return 0;
		}


        // Get both
		public int[] GetKeyAndButtonIndex(String name)
        {
            foreach (Button kv in keys)
            {
                if (kv.name.Equals(name))
                {
					return new int[] { kv.key, kv.button };
                }
            }
			return new int[]{0, 0};
        }
    }
}
