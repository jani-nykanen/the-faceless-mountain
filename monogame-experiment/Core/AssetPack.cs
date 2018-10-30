using System;
using System.Collections.Generic;
using System.Xml.Linq;


namespace monogame_experiment.Desktop.Core
{
	// An asset pack used to load a pack of assets
    public class AssetPack
    {

		// Bitmaps
		private List<KeyValuePair<Bitmap>> bitmaps;
        // Tilemaps
        private List<KeyValuePair<Tilemap>> tilemaps;


		// Constructor
        public AssetPack(String path)
        {
			// Assign lists
			bitmaps = new List<KeyValuePair<Bitmap>>();
            tilemaps = new List<KeyValuePair<Tilemap>>();

            // Open document
            XElement doc = XElement.Load(path);

			// Paths
			String bmpPath = "";
            String mapPath = "";

            // Get attributes
			if(doc.HasAttributes) 
			{
				// Get asset paths
				bmpPath = doc.Attribute("bitmap_path").Value;
                mapPath = doc.Attribute("tilemap_path").Value;
			}

            // Go through nodes
            String name, fpath, type;
			foreach (XElement e in doc.Elements())
			{
				// Get name & path
				name = e.Attribute("name").Value;
				fpath = e.Attribute("path").Value;

                // Load bitmaps
                type = e.Name.ToString();
                if (type == "bitmap")
                {
                    bitmaps.Add(new KeyValuePair<Bitmap>(name, new Bitmap(bmpPath + fpath)));
                }
                else if(type == "tilemap")
                {
                    tilemaps.Add(new KeyValuePair<Tilemap>(name, new Tilemap(mapPath + fpath)));
                }
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


        // Get a tilemap
        // TODO: Generic method for these two?
        public Tilemap GetTilemap(String name)
        {
            // Go through bitmaps & compare names
            foreach (var b in tilemaps)
            {
                if (b.key.Equals(name))
                {
                    return b.value;
                }
            }

            return null;
        }
    }
}
