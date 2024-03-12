using Godot;
using System;

namespace Abbaye.script.content;
public class SpawnInfo {
    public readonly int tstart;
    public readonly int tend;
    public readonly string path;
    public readonly PackedScene scene;
    public readonly int wsize;
    public readonly int wdelay;

    public int delaycounter = 0;


    public SpawnInfo(int ntstart, int ntend, string npath, int nwsize, int nwdelay) :
        this(ntstart, ntend, GD.Load<PackedScene>(npath), nwsize, nwdelay) { }

    public SpawnInfo(PackedScene nscene, int nwsize, int nwdelay) :
        this(0, 99999, nscene, nwsize, nwdelay) { }
    public SpawnInfo(int ntstart, int ntend, PackedScene nscene, int nwsize, int nwdelay) {
        tstart = ntstart;
        tend = ntend;
        path = nscene.ResourcePath;
        scene = nscene;
        wsize = nwsize;
        wdelay = Math.Min(1, nwdelay);

    }
}

public class SpawnRound {

    public readonly int duration;
    public readonly SpawnInfo[] infos;

    public SpawnRound(int ndur) : this(ndur, Array.Empty<SpawnInfo>()) {

    }
    public SpawnRound(int ndur, params SpawnInfo[] ninfos) {
        duration = ndur;
        infos = ninfos;
    }




}
