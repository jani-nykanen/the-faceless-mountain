using System;
using System.Collections.Generic;

using System.Xml.Linq;


namespace monogame_experiment.Desktop.Core
{
    // A tilemap
    public class Tilemap
    {

        // Dimensions
        private int width;
        private int height;

        // Layers
        private List<int[]> layers;


        // Remove white spaces
        private static String RemoveWhitespaces(String input)
        {
            return input.Replace(" ", String.Empty).
                        Replace("\t", String.Empty).
                        Replace("\n", String.Empty).
                        Replace("\r\n", String.Empty);
        }


        // Parse layer
        private int[] ParseLayer(String data)
        {
            int[] layer = new int[width * height];
            String[] values = RemoveWhitespaces(data).Split(',');

            int i = 0;
            foreach(String s in values)
            {
                layer[i++] = int.Parse(s);
            }

            return layer;
        }


        // Parse XML
        private void ParseXML(String path)
        {
            // Initialize layer list
            layers = new List<int[]>();

            // Open document
            XElement doc = XElement.Load(path);

            // Get attributes
            width = int.Parse(doc.Attribute("width").Value);
            height = int.Parse(doc.Attribute("height").Value);

            // Get layers
            String layerData = "";
            foreach(XElement elem in doc.Elements())
            {
                if (elem.Name.ToString() != "layer")
                    continue;

                // Parse layer
                layerData = elem.Element("data").Value.ToString();
                layers.Add(ParseLayer(layerData));
            }

        }


        // Constructor (with path)
        // Parses a Tiled map file
        public Tilemap(String path)
        {
            ParseXML(path);
        }


        // Get width
        public int GetWidth()
        {
            return width;
        }


        // Get height
        public int GetHeight()
        {
            return height;
        }


        // Get tile
        public int GetTile(int layer, int x, int y)
        {
            if (layer < 0 || layer >= layers.Count
                || x < 0 || y < 0 || x >= width || y >= height)
                return -1;

            return layers[layer][y * width + x];
        }
    }
}
