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
        // Samples
        private List<KeyValuePair<Sample>> samples;


        // Constructor
        public AssetPack(String path)
        {
			// Assign lists
			bitmaps = new List<KeyValuePair<Bitmap>>();
            tilemaps = new List<KeyValuePair<Tilemap>>();
            samples = new List<KeyValuePair<Sample>>();

            // Open document
            XElement doc = XElement.Load(path);

			// Paths
			String bmpPath = "";
            String mapPath = "";
            String aPath = "";

            // Get attributes
			if(doc.HasAttributes) 
			{
				// Get asset paths
				bmpPath = doc.Attribute("bitmap_path").Value;
                mapPath = doc.Attribute("tilemap_path").Value;
                aPath = doc.Attribute("audio_path").Value;
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
                else if (type == "sample")
                {
                    samples.Add(new KeyValuePair<Sample>(name, new Sample(aPath + fpath)));
                }
            }

        }


        // Get any asset
        private T GetAsset<T> (String name, List<KeyValuePair<T>> arr)
        {
            foreach(var x in arr)
            {
                if(x.key.Equals(name))
                {
                    return x.value;
                }
            }

            return default(T);
        }


        // Get a bitmap
        public Bitmap GetBitmap(String name)
		{
            return GetAsset<Bitmap>(name, bitmaps);
		}


        // Get a tilemap
        public Tilemap GetTilemap(String name)
        {
            return GetAsset<Tilemap>(name, tilemaps);
        }


        // Get a sample
        public Sample GetSample(String name)
        {
            return GetAsset<Sample>(name, samples);
        }


        // Destroy
        public void Destroy()
        {
            foreach(KeyValuePair<Sample> s in samples)
            {
                s.value.Destroy();
            }
        }
    }
}
