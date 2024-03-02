using Abbaye.misc;
using Godot;
using System;
using System.Runtime.CompilerServices;



public partial class EnemySmall1 : CharacterBody2D {
	[Export]
	public float Speed = 20f;
	[Export]
	public int Hp = 10;
	public Player? player;
	public Sprite2D? sprite;
	public Timer? wtimer; //walk anim timer

	public override void _Ready() {
		player = this.GetNodeAs<Player>(() => GetTree().GetFirstNodeInGroup("player"));
		sprite = this.GetNodeAs<Sprite2D>("Sprite2D");
		wtimer = this.GetNodeAs<Timer>("WalkAnimTimer");
	}

	public override void _PhysicsProcess(double delta) {
		Vector2 dir = GlobalPosition.DirectionTo(player!.GlobalPosition);
		Velocity = dir * Speed;

		sprite!.FlipH = dir.X < 0;
		sprite!.AnimOnTimer(wtimer!, true);

		MoveAndSlide();
	}

	private void OnHurtboxHurt(int damage) {
		Hp -= damage;
		if (Hp < 0) {
			QueueFree();
		}
	}
}






