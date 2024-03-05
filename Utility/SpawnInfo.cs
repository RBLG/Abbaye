using Godot;
using System;

public class SpawnInfo {
    public int tstart;
    public int tend;
    public string path;
    public PackedScene? scene;
    public int wsize;
    public int wdelay;

    public int delaycounter = 0;


    public SpawnInfo(int ntstart, int ntend, string npath, int nwsize, int nwdelay) {
        tstart = ntstart;
        tend = ntend;
        path = npath;
        scene = GD.Load<PackedScene>(npath);
        wsize = nwsize;
        wdelay = nwdelay;
    }
}
