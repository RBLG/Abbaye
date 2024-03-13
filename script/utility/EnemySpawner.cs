using Abbaye.script.content;
using Abbaye.script.misc;
using Godot;
using System;
using System.Collections.Generic;

namespace Abbaye.script;
public partial class EnemySpawner : Node2D {


    public static readonly PackedScene ENEMY_SPARK = GD.Load<PackedScene>("res://scenes/enemies/enemy.tscn");
    public static readonly PackedScene ENEMY_FAIRY = GD.Load<PackedScene>("res://scenes/enemies/xp_fairy.tscn");
    public static readonly PackedScene ENEMY_LONGLEG = GD.Load<PackedScene>("res://scenes/enemies/longleg.tscn");
    public static readonly PackedScene ENEMY_BRAIN = GD.Load<PackedScene>("res://scenes/enemies/brain.tscn");
    public static readonly PackedScene ENEMY_TALL_SHROOM = GD.Load<PackedScene>("res://scenes/enemies/tall_shroom.tscn");
    public static readonly PackedScene ENEMY_SPINDA = GD.Load<PackedScene>("res://scenes/enemies/spinda.tscn");
    public static readonly PackedScene ENEMY_RUNNER = GD.Load<PackedScene>("res://scenes/enemies/runner.tscn");
    public static readonly PackedScene ENEMY_TALL_SOFTHEAD = GD.Load<PackedScene>("res://scenes/enemies/tall_softhead.tscn");
    public static readonly PackedScene ENEMY_THIN_SPINDA = GD.Load<PackedScene>("res://scenes/enemies/thin_spinda.tscn");
    public static readonly PackedScene ENEMY_TALL_SPINDA = GD.Load<PackedScene>("res://scenes/enemies/tall_spinda.tscn");
    public static readonly PackedScene ENEMY_GIANT_EYE = GD.Load<PackedScene>("res://scenes/enemies/giant_eye.tscn");
    public static readonly PackedScene ENEMY_GIANT_FACE = GD.Load<PackedScene>("res://scenes/enemies/giant_face.tscn");


    //

    public SpawnRound[] rounds = new SpawnRound[] {
        new(30,new SpawnData[]{
            new(ENEMY_FAIRY   , 02, 03),
        }),
        new(40,new SpawnData[]{
            new(20,ENEMY_FAIRY, 01, 06),
            new(ENEMY_LONGLEG , 02, 02),
        }),
        new(10),
        new(50,new SpawnData[]{
            new(ENEMY_LONGLEG , 03, 02),
            new(ENEMY_BRAIN, 05, 02),
        }),
        new(10),
        new(50,new SpawnData[]{
            new(ENEMY_LONGLEG , 04, 02),
            new(ENEMY_TALL_SHROOM, 02, 10),
        }),
        new(10),
        new(50,new SpawnData[]{
            new(ENEMY_SPINDA, 03, 02),
            new(ENEMY_LONGLEG, 01, 10),
        }),
        new(50,new SpawnData[]{
            new(ENEMY_SPINDA, 04, 02),
            new(ENEMY_RUNNER, 05, 02),
        }),
        new(50,new SpawnData[]{
            new(ENEMY_SPINDA, 05, 02),
            new(ENEMY_TALL_SOFTHEAD, 02, 10),
        }),
        new(50,new SpawnData[]{
            new(20,ENEMY_THIN_SPINDA, 07, 02),
            new(ENEMY_RUNNER, 7, 8),
        }),
        new(10),
        new(50,new SpawnData[]{
            new(20,ENEMY_THIN_SPINDA, 07, 02),
            new(ENEMY_TALL_SPINDA, 01, 10),
        }),
        new(1,new SpawnData[]{
            new(ENEMY_GIANT_EYE, 01, 3000),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_RUNNER, 08, 1),
            new(ENEMY_BRAIN, 08, 1),
        }),
        new(120,new SpawnData[]{
            new(ENEMY_TALL_SHROOM, 07, 01),
            new(20,ENEMY_THIN_SPINDA, 05, 01),
            new(ENEMY_TALL_SOFTHEAD, 03, 10),
            new(ENEMY_TALL_SPINDA, 03, 10),
        }),
        new(120,new SpawnData[]{
            new(ENEMY_TALL_SOFTHEAD, 10, 02),
            new(ENEMY_TALL_SPINDA, 10, 02),
        }),
        new(1,new SpawnData[]{
            new(ENEMY_GIANT_FACE, 01, 3000),
        }),
        new(120,new SpawnData[]{
            new(ENEMY_GIANT_EYE, 04, 03),
            new(ENEMY_TALL_SOFTHEAD, 04, 01),
            new(ENEMY_TALL_SPINDA, 04, 01),
        }),
        new(60,new SpawnData[]{
            new(ENEMY_GIANT_EYE, 04, 01),
            new(ENEMY_TALL_SOFTHEAD, 01, 01),
            new(ENEMY_TALL_SPINDA, 10, 01),
            new(ENEMY_GIANT_FACE, 04, 01),
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
            for (int counter = 0; counter < data.wsize; counter++) {
                var enemy_spawn = data.scene!.Instantiate<Node2D>();
                enemy_spawn!.GlobalPosition = GetRandomPosition();
                float col = (float)GD.RandRange(0.7, 1);
                enemy_spawn.Modulate = new(col, col, col);
                AddChild(enemy_spawn);
            }
        }
        time += 1;
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

















}
