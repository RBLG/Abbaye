using Abbaye.script.content;
using Abbaye.script.misc;
using Godot;
using System;
using System.Collections.Generic;

namespace Abbaye.script;
public partial class EnemySpawner : Node2D {


    public static readonly PackedScene ENEMY_SPARK = GD.Load<PackedScene>("res://scenes/enemies/enemy.tscn");
    public static readonly PackedScene ENEMY_FAIRY = GD.Load<PackedScene>("res://scenes/enemies/xp_fairy.tscn");
    public static readonly PackedScene ENEMY_SPINDA = GD.Load<PackedScene>("res://scenes/enemies/spinda.tscn");
    public static readonly PackedScene ENEMY_RUNNER = GD.Load<PackedScene>("res://scenes/enemies/runner.tscn");
    public static readonly PackedScene ENEMY_TALL_SOFTHEAD = GD.Load<PackedScene>("res://scenes/enemies/tall_softhead.tscn");


    //

    public SpawnRound[] rounds = new SpawnRound[] {
        new(30,new SpawnInfo[]{
            new(ENEMY_FAIRY, 01, 03),
        }),
        new(50,new SpawnInfo[]{
            new(0,20,ENEMY_FAIRY, 01, 10),
            new(ENEMY_SPINDA, 01, 10),
        }),
        new(10),
        new(50,new SpawnInfo[]{
            new(ENEMY_SPINDA, 01, 10),
            new(0,20,ENEMY_RUNNER, 06, 10),
        }),
        new(10),
        new(40,new SpawnInfo[]{
            new(ENEMY_SPINDA, 01, 10),
            new(ENEMY_TALL_SOFTHEAD, 01, 10),
        }),
        new(20),
        new(60,new SpawnInfo[]{
            new(ENEMY_SPINDA, 01, 06),
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

        SpawnInfo[] spawns = rounds[round].infos;
        foreach (SpawnInfo ene in spawns) {
            if (!(ene.tstart <= time && time <= ene.tend)) {
                continue;
            }
            if (ene.delaycounter < ene.wdelay - 1) {
                ene.delaycounter += 1;
                continue;
            }
            ene.delaycounter = 0;
            for (int counter = 0; counter < ene.wsize; counter++) {
                var enemy_spawn = ene.scene!.Instantiate<Node2D>();
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
