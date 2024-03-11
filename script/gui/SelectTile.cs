using Abbaye.script.content;
using Godot;
using System;

namespace Abbaye.script;
public partial class SelectTile : Panel {

    SocketMap? sockets;

    public AttackPattern? pat;
    public AttackType optype = AttackType.Dark;
    public int level = 0;

    public override void _Ready() {
        sockets = GetNode<SocketMap>("%SocketMap");


        sockets.OnClick += OnInput;
        UpdateMainLayer();
    }

    public void SetStuff(AttackPattern npat, AttackType noptype, int nlvl) {
        pat = npat;
        optype = noptype;
        level = nlvl;
    }

    public Vector2I? lpos = null;
    public int[] ltoadd = Array.Empty<int>();


    public override void _Process(double delta) {
        if (!enabled) {
            return;
        }
        if (sockets != null && pat != null) {
            Vector2 mpos = GetViewport().GetMousePosition();
            Vector2I tpos = sockets.LocalToMap(sockets.ToLocal(mpos));
            //TileData? data = sockets.GetCellTileData(0, tile_pos);

            if (tpos.Equals(lpos)) {
                return;
            }
            int half = AttackPattern.Size / 2;

            ClearSocketsLayer1();
            lpos = tpos;
            if (!IsVecInMapBounds(tpos)) {
                ltoadd = Array.Empty<int>();
                lpos = null;
                return;
            }

            int index = pat.board.GetIndex(tpos.X, tpos.Y);

            int[] poss = GetToAddFromType(optype, index);
            ltoadd = poss;

            Vector2I tile = GetAtlasVectorFromType(optype);

            foreach (int pos in poss) {
                var (x, y) = pat.board.GetXYFromIndex(pos);
                sockets.SetCell(1, new(x, y), 0, tile);
            }


            //TileData tile_data = sockets.GetCellTileData(0, tile_pos);
            //tile_data.Modulate = new Color(0.7f, 0.7f, 0.7f);




        }
    }

    private void OnInput() {
        if (!enabled) {
            return;
        }
        int half = AttackPattern.Size / 2;
        var mpos = GetViewport().GetMousePosition();
        Vector2I tpos = sockets!.LocalToMap(sockets.ToLocal(mpos));
        if (!IsVecInMapBounds(tpos)) {
            return;
        }
        if (ltoadd == null) {
            return;
        }
        OnSocketChosen?.Invoke(ltoadd);
        this.QueueFree();
    }

    public delegate void OnSocketChosenEventHandler(int[] toadd);
    public OnSocketChosenEventHandler? OnSocketChosen { get; set; }

    bool enabled = false;

    public void Enable() {
        enabled = true;
    }

    public void ClearSocketsLayer1() {
        sockets!.RemoveLayer(1);
        sockets.AddLayer(1);
    }

    public void UpdateMainLayer() {
        int half = AttackPattern.Size / 2;
        for (int itx = -half; itx <= half; itx++) {
            for (int ity = -half; ity <= half; ity++) {
                int atypint = pat!.board[itx, ity];
                AttackType atyp = GetAttackTypeFromInt(atypint);
                if (atypint != 0) {
                    sockets!.SetCell(0, new(itx, ity), 0, GetAtlasVectorFromType(atyp));
                }
            }
        }
    }

    private AttackType GetAttackTypeFromInt(int atypint) {
        return atypint switch {
            2 => AttackType.Dark,
            3 => AttackType.Fire,
            1 => AttackType.Psy_,
            _ => AttackType.None,
        };
    }

    public Vector2I GetAtlasVectorFromType(AttackType atype) {
        return atype switch {
            AttackType.Dark => new(3, 0),
            AttackType.Fire => new(5, 0),
            AttackType.Psy_ => new(4, 0),
            _ => new(),
        };
    }

    public int[] GetToAddFromType(AttackType atype, int index) {
        return atype switch {
            AttackType.Dark => UpgradePatterns.GetUpgradeDark(pat!, level, index),
            AttackType.Fire => UpgradePatterns.GetUpgradeFire(pat!, level, index),
            AttackType.Psy_ => UpgradePatterns.GetUpgradePsy_(pat!, level, index),
            _ => Array.Empty<int>(),
        };
    }

    public bool IsVecInMapBounds(Vector2I tpos) {
        int half = AttackPattern.Size / 2;
        return -half <= tpos.X && tpos.X <= half && -half <= tpos.Y && tpos.Y <= half;
    }


}
