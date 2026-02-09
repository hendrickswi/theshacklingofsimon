using System.Collections.Generic;

namespace TheShacklingOfSimon.Sprites;

public class SpriteDataRoot
{
    public List<SpriteData> sprites { get; set; }
    
    // Nested class to hold data of each sprite
    public class SpriteData
    {
        public string name { get; set; }
        public List<FrameData> frames { get; set; }
    }

    // Nested class to hold each frame 
    public class FrameData
    {
        public int x { get; set; }
        public int y { get; set; }
        public int w { get; set; }
        public int h { get; set; }
    }
}