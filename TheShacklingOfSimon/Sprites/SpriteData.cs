using System.Collections.Generic;

namespace TheShacklingOfSimon.Sprites;

// The root of the JSON structure.
public class SpriteDataRoot
{
    public List<SpriteData> Sprites { get; set; }
}

// Collection of frames given a name
public class SpriteData
{
        public string Name { get; set; }
        public List<FrameData> Frames { get; set; }
}

// Position for each frame
public class FrameData
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
}