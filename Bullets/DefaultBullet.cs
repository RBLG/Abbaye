using Abbaye.misc;
using Godot;
using System;
using System.Security.Cryptography;

public partial class DefaultBullet : Area2D {
	int level = 1;
	int hp = 1;
	int speed = 100;
	int damage = 5;
	int knock_amount = 100;
	float attack_size = 1.0f;

	public Vector2 dir = Vector2.Zero;

	Player? player;
	Timer? timer;

	public override void _Ready() {
		player = this.GetFirstNodeInGroupAs<Player>("player");
		timer = this.GetNodeAs<Timer>("Timer");
		timer.Timeout += Timeout;
		//Rotation = dir.Angle() + (float)(135 * Math.PI / 180);
	}

	public override void _PhysicsProcess(double delta) {
		Position += dir * speed * (float)delta;
	}

	public void EnemyHit(int charge) {
		hp -= charge;
		if (hp <= 0) {
			this.QueueFree();
		}
	}

	public void Timeout() {
		this.QueueFree();
	}




}
