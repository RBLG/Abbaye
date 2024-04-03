using Godot;
using System;

namespace Abbaye.script.content;
public class SpawnData {
    public readonly int tstart;
    public readonly int tend;
    public readonly string path;
    public readonly PackedScene scene;
    public readonly int wsize;
    public readonly int wdelay;

    public int delaycounter = 0;

    public SpawnData(PackedScene nscene) :
        this(nscene, 1) { }
    public SpawnData(PackedScene nscene, int nwsize) :
        this(nscene, nwsize, 1) { }
    public SpawnData(PackedScene nscene, int nwsize, int nwdelay) :
        this(99999, nscene, nwsize, nwdelay) { }
    public SpawnData(int ntend, PackedScene nscene, int nwsize, int nwdelay) :
        this(0, ntend, nscene, nwsize, nwdelay) { }
    public SpawnData(int ntstart, int ntend, PackedScene nscene, int nwsize, int nwdelay) {
        tstart = ntstart;
        tend = ntend;
        path = nscene.ResourcePath;
        scene = nscene;
        wsize = nwsize;
        wdelay = Math.Max(1, nwdelay);

    }
}

public class SpawnRound {

    public readonly int duration;
    public readonly SpawnData[] datas;

    public SpawnRound(SpawnData ninfo) : this(1, new SpawnData[] { ninfo }) { }

    public SpawnRound(int ndur, SpawnData[] ninfos) {
        duration = ndur;
        datas = ninfos;
    }




}
