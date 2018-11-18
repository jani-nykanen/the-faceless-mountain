#!/bin/sh
cd "$(dirname "$0")"
convert ^ tileset.png ^ -bordercolor None -trim +repage ^ -crop 64x64 +repage ^ -border 1 ^ +append ^ -crop 1056x66 +repage ^ -append ^ -trim +repage ^ output.png
mv output.png ../monogame-experiment/Assets/Bitmaps/tileset.png
