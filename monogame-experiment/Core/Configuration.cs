using System;
using System.Collections.Generic;

using System.Xml.Linq;

namespace monogame_experiment.Desktop.Core
{
	// Simple configuration structure
	public class Configuration
    {      

		// Initial window size
		public int winWidth = 800;
		public int winHeight = 600;

		// Is fullscreen
		public bool fullscreen = false;

		// Window caption
		public String caption = "Game";

		// Framerate
		public int frameRate = 60;
      
        // Other parameters
		private List<KeyValuePair<String>> otherParams = new List<KeyValuePair<String>> ();


        // Set a parameter
        public void SetParam(String name, String value)
		{
			
			switch(name)
			{
				case "window_width":
					winWidth = int.Parse(value);
					break;

				case "window_height":
					winHeight = int.Parse(value);
                    break;

				case "fullscreen":
					fullscreen = bool.Parse(value);
                    break;

				case "framerate":
					frameRate = int.Parse(value);
                    break;

				case "caption":
					caption = value;
                    break;

                
				default:

					// Add to "other params"
					otherParams.Add(new KeyValuePair<String>(name, value));

					break;
			}
		}


        // Get other param
        public String GetOtherParam(String name)
		{
			foreach(KeyValuePair<String> p in otherParams) 
			{
				if(name.Equals(p.key))
				{
					return p.value;
				}
			}

			return "";
		}


        // Parse XML configuration file
		// TODO: Generic XML parser?
		static public Configuration ReadXML(String path) 
		{
			Configuration conf = new Configuration();

			// Open document
            XElement doc = XElement.Load(path);

            // Go through nodes
			String key, value;
            foreach (XElement e in doc.Elements())
            {
                // Get key & value
				key = e.Attribute("key").Value;
				value = e.Attribute("value").Value;

				// Add a parameter
				conf.SetParam(key, value);
            }

			return conf;
		}
    }
}
