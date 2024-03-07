using Abbaye.misc;
using Godot;
using System;
using System.Security.Cryptography;

public partial class DefaultBullet : Node2D {
	[Export]
	int speed = 100;
	[Export]
	int damage = 10; //TODO
	int knock_amount = 100;
	//float attack_size = 1.0f;

	public Vector2 dir = Vector2.Zero;

	Timer? timer;
	Hitbox? hitbox;

	public override void _Ready() {
		timer = this.GetNodeAs<Timer>("Timer");
		hitbox = this.GetNode<Hitbox>("Hitbox");

		hitbox.Damage = damage;
		hitbox.OnHit += OnHit;
		timer.Timeout += Timeout;
	}

	public override void _PhysicsProcess(double delta) {
		Position += dir * speed * (float)delta;
	}

	public void OnHit() {
		/*hp -= charge;
		if (hp <= 0) {
			this.QueueFree();
		}*/
		this.QueueFree();
	}

	public void Timeout() {
		this.QueueFree();
	}




}
