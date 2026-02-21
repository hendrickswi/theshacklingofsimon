using System.Collections.Generic;

namespace TheShacklingOfSimon.Sprites.Factory.Data;
// Collection of frames given a name
public class SpriteData
{
    public string Name { get; set; }
    public List<FrameData> Frames { get; set; }
}