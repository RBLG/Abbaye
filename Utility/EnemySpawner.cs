using Abbaye.misc;
using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D {


	public static readonly string ENEMY_SMALL_1 = "res://enemy/enemy.tscn";


	public SpawnInfo[] spawns = new SpawnInfo[] {
		//
		new(000, 005, ENEMY_SMALL_1, 01, 00),
		new(005, 120, ENEMY_SMALL_1, 20, 04),
	};

	Player? player;
	Timer? timer;

	public override void _Ready() {
		player = this.GetFirstNodeInGroupAs<Player>("player");
		timer = this.GetNodeAs<Timer>("Timer");
		timer.Timeout += OnTimerTimeout;
	}

	int time = 0;

	public void OnTimerTimeout() {
		time += 1;
		foreach (SpawnInfo ene in spawns) {
			if (time >= ene.tstart && time <= ene.tend) {
				ene.delaycounter += 1;
			} else {
				ene.delaycounter = 0;
				for (int counter = 0; counter < ene.wsize; counter++) {
					var enemy_spawn = ene.scene!.Instantiate() as Node2D ?? throw new Exception();
					enemy_spawn!.GlobalPosition = GetRandomPosition();
					AddChild(enemy_spawn);
				}
			}
		}

	}

	readonly Random rand = new();

	public Vector2 GetRandomPosition() {
		Vector2 vpr = GetViewportRect().Size * (float)GD.RandRange(1.1f, 1.4f);
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
