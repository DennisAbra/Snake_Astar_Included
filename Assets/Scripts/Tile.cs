using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public GameObject gameObjectdata;
    public Tile next;

    public Tile(GameObject d)
    {
        gameObjectdata = d;
        next = null;
    }
    
}
