using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace monogame_experiment.Desktop.Core
{
	// An asset pack used to load a pack of assets
    public class AssetPack
    {

        // A key-value pair for assest
		private struct KeyValuePair<T>
		{
			public T value;
			public String key;

            // Constructor
			public KeyValuePair(T value, String key)
			{
				this.value = value;
				this.key = key;
			}
		}


		// Bitmaps
		private List<KeyValuePair<Bitmap>> bitmaps;


		// Constructor
        public AssetPack(String path)
        {
			// Assign bitmap list
			bitmaps = new List<KeyValuePair<Bitmap>>();

			// Open document
			XElement doc = XElement.Load(path);

			// Paths
			String bmpPath = "";

            // Get attributes
			if(doc.HasAttributes) 
			{
				// Get asset paths
				bmpPath = doc.Attribute("bitmap_path").Value;
			}

			// Go through nodes
			String name, fpath;
			foreach (XElement e in doc.Elements())
			{
				// Get name & path
				name = e.Attribute("name").Value;
				fpath = e.Attribute("path").Value;
            
				// Load bitmaps
				bitmaps.Add(new KeyValuePair<Bitmap>(new Bitmap(bmpPath + fpath), name));
			}

        }


        // Get a bitmap
        public Bitmap GetBitmap(String name)
		{
			// Go through bitmaps & compare names
			foreach(var b in bitmaps)
			{
				if(b.key.Equals(name))
				{
					return b.value;
				}
			}
            
			return null;
		}
    }
}
