using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventList
{
    private static Dictionary<string, int> regionpairs = new Dictionary<string, int>()
    {
        {"forest", 0 },
        {"plain", 1 },
        {"mountain", 2 },
        {"town", 3 }
    };

    private (string, double)[] forest = {("none", 1.0f), ("scenario", 0.8f), ("sunset", 0.4)};
    private (string, double)[] plain = { ("none", 1.0f), ("scenario", 0.8f) };

    
    public (string, double)[] RegionFind(string name)
    {
        //Debug.Log(regionpairs[name]);
        switch (regionpairs[name])
        {
            case 0: return forest;
            case 1: return plain;
            default: return null;
        }
    }

}
