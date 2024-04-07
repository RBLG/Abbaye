using Abbaye.script.content;
using Abbaye.script.misc;
using Godot;
using System;
using System.Collections.Generic;

namespace Abbaye.script;
public partial class EnemySpawner : Node2D {

    public const int MAX_ENEMY_AMOUNT = 100;

    public static readonly PackedScene ENEMY_SPARK__ = GD.Load<PackedScene>("res://scenes/enemies/enemy.tscn");
    public static readonly PackedScene ENEMY_FAIRY__ = GD.Load<PackedScene>("res://scenes/enemies/xp_fairy.tscn");
    public static readonly PackedScene ENEMY_LONGLEG = GD.Load<PackedScene>("res://scenes/enemies/longleg.tscn");
    public static readonly PackedScene ENEMY_BRAIN__ = GD.Load<PackedScene>("res://scenes/enemies/brain.tscn");
    public static readonly PackedScene ENEMY_SPINDA_ = GD.Load<PackedScene>("res://scenes/enemies/spinda.tscn");
    public static readonly PackedScene ENEMY_RUNNER_ = GD.Load<PackedScene>("res://scenes/enemies/runner.tscn");
    public static readonly PackedScene ENEMY_TALLSHROOM__ = GD.Load<PackedScene>("res://scenes/enemies/tall_shroom.tscn");
    public static readonly PackedScene ENEMY_TALLSOFTHEAD = GD.Load<PackedScene>("res://scenes/enemies/tall_softhead.tscn");
    public static readonly PackedScene ENEMY_THIN_SPINDA_ = GD.Load<PackedScene>("res://scenes/enemies/thin_spinda.tscn");
    public static readonly PackedScene ENEMY_TALL_SPINDA_ = GD.Load<PackedScene>("res://scenes/enemies/tall_spinda.tscn");
    public static readonly PackedScene ENEMY_GIANT_EYE___ = GD.Load<PackedScene>("res://scenes/enemies/giant_eye.tscn");
    public static readonly PackedScene ENEMY_GIANT_FACE__ = GD.Load<PackedScene>("res://scenes/enemies/giant_face.tscn");
    public static readonly PackedScene ENEMY_SMALL_EYE___ = GD.Load<PackedScene>("res://scenes/enemies/small_eye.tscn");
    public static readonly PackedScene ENEMY_GOD_FACE____ = GD.Load<PackedScene>("res://scenes/enemies/god_face.tscn");

    public SpawnRound[] rounds = new SpawnRound[] {
        //new(1160,new SpawnData[]{new(ENEMY_LONGLEG, 10)}),
        new(10,new SpawnData[]{
            new(ENEMY_FAIRY__, 5, 3),
        }),
        new(40,new SpawnData[]{
            new(ENEMY_FAIRY__, 1, 6),
            new(ENEMY_LONGLEG, 1),
        }),
        new(new(ENEMY_SPINDA_, 2,999)),
        new(60,new SpawnData[]{
            new(ENEMY_LONGLEG, 1),
            new(ENEMY_BRAIN__, 3),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_LONGLEG, 2),
            new(ENEMY_TALLSHROOM__, 3),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_SPINDA_, 1),
            new(ENEMY_BRAIN__, 4),
            new(20,21,ENEMY_TALLSOFTHEAD,1,999), //BOSS: TALLSOFTHEAD (3min20)
        }),
        new(60,new SpawnData[]{
            new(ENEMY_SPINDA_, 3),
            new(ENEMY_RUNNER_, 3),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_SPINDA_, 3),
            new(ENEMY_TALLSOFTHEAD, 2, 10),
            new(ENEMY_BRAIN__, 1),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_THIN_SPINDA_, 4),
            new(ENEMY_RUNNER_, 3, 4),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_THIN_SPINDA_, 7, 2),
            new(ENEMY_TALL_SPINDA_, 2, 7),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_RUNNER_, 8),
            new(ENEMY_BRAIN__, 5),
            new(20,21,ENEMY_GIANT_EYE___,1,999), //BOSS: GIANT EYE (8min20)
            new(20,21,ENEMY_SMALL_EYE___,3,999),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_TALLSHROOM__, 7),
            new(20,ENEMY_THIN_SPINDA_, 5, 1),
            new(ENEMY_TALLSOFTHEAD, 3, 10),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_TALLSOFTHEAD, 5),
            new(ENEMY_TALL_SPINDA_, 5),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_THIN_SPINDA_, 8),
            new(ENEMY_SMALL_EYE___, 5),
            new(20,21,ENEMY_GIANT_FACE__,1,999), //BOSS: GIANT FACE (11min20)
        }),
        new(60,new SpawnData[]{
            new(ENEMY_GIANT_EYE___, 4, 3),
            new(ENEMY_TALLSOFTHEAD, 4),
            new(ENEMY_TALL_SPINDA_, 4),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_GIANT_EYE___, 2),
            new(ENEMY_SMALL_EYE___, 8),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_GIANT_EYE___, 3),
            new(ENEMY_TALL_SPINDA_, 8),
            new(20,21,ENEMY_GOD_FACE____,1,999), //BOSS: GOD FACE (14min20)
        }),
        new(60,new SpawnData[]{
            new(ENEMY_GIANT_EYE___, 6),
            new(ENEMY_GIANT_FACE__, 5),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_GIANT_FACE__, 4),
            new(ENEMY_SMALL_EYE___, 8),
        }),
        new(60,new SpawnData[]{ // (18min)
            new(ENEMY_GIANT_EYE___, 4),
            new(ENEMY_GIANT_FACE__, 6),
            new(ENEMY_TALL_SPINDA_, 6),
        }),

    };

    Player? player;
    Timer? timer;

    public override void _Ready() {
        player = this.GetFirstNodeInGroupAs<Player>("player");
        timer = this.GetNodeAs<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;
    }

    int time = 0;
    int round = 0;

    public bool cd = false;

    public void OnTimerTimeout() {
        if (cd) {
            return;
        }
        cd = true;
        SetDeferred("cd", false);

        if (rounds[round].duration < time) {
            if (round < rounds.Length - 1) {
                round += 1;
            }
            time = 0;
        }

        SpawnData[] datas = rounds[round].datas;
        foreach (SpawnData data in datas) {
            if (time < data.tstart || data.tend < time) {
                continue;
            }
            if (0 < data.delaycounter) {
                data.delaycounter -= 1;
                continue;
            }
            data.delaycounter = data.wdelay - 1;

            //MakeRoomForXEntity(data.wsize);
            for (int counter = 0; counter < data.wsize; counter++) {
                var enemy_spawn = data.scene!.Instantiate<Node2D>();
                enemy_spawn!.GlobalPosition = GetRandomPosition();
                float col = (float)GD.RandRange(0.7, 1);
                enemy_spawn.Modulate = new(col, col, col);
                AddChild(enemy_spawn);
            }
        }
        time += 1;

        //GD.Print("enemies: " + GetChildCount());
    }

    readonly Random rand = new();

    public Vector2 GetRandomPosition() {
        Vector2 vpr = GetViewportRect().Size * (float)GD.RandRange(1.05f, 1.2f);
        Vector2 vpr2 = vpr / 2;
        var ppos = player!.GlobalPosition;
        bool horv = rand.Next(2) - 1 < 0; //on horizontal sides or vertical sides
        bool norp = rand.Next(2) - 1 < 0; // on negative or positive side

        Vector2 pos = new(0, 0);
        if (horv) {
            pos.X = norp ? vpr2.X : -vpr2.X;
            pos.Y = (float)GD.RandRange(-vpr2.Y, vpr2.Y);
        } else {
            pos.X = (float)GD.RandRange(-vpr2.X, vpr2.X);
            pos.Y = norp ? vpr2.Y : -vpr2.Y;
        }
        return ppos + pos;
    }

    public Node2D[] GetXFurtherChildren(int amount) {
        var children = GetChildren();

        List<(float, Node2D)> lasts = new(amount + 1);
        foreach (var pchild in children) {
            if (pchild is not Node2D) {
                continue;
            }
            Node2D child = (Node2D)pchild;
            float len = child.GlobalPosition.DistanceSquaredTo(player!.GlobalPosition);
            int it = 0;
            foreach (var (olen, ochild) in lasts) {
                if (olen < len) {
                    break;
                }
                it++;
            }
            lasts.Insert(it, (len, child));
            if (amount < lasts.Count) {
                lasts.RemoveAt(lasts.Count - 1);
            }
        }

        Node2D[] rtn = new Node2D[lasts.Count];
        int it2 = 0;
        foreach (var (_, child) in lasts) {
            rtn[it2] = child;
            it2++;
        }

        return rtn;
    }

    public void MakeRoomForXEntity(int amount) {
        if ((GetChildCount() + amount) < MAX_ENEMY_AMOUNT) {
            return;
        }
        Node2D[] nodes = GetXFurtherChildren(amount);
        foreach (Node2D node in nodes) {
            node.QueueFree();
        }
    }

















}
