using Abbaye.misc;
using Godot;
using System;

public partial class Player : CharacterBody2D {
	[Export]
	public float Speed = 60f;
	[Export]
	public int Hp = 100;
	public Sprite2D? sprite;
	public Timer? wtimer; //walk anim timer
	public Hurtbox? hurtbox;

	public override void _Ready() {
		sprite = this.GetNodeAs<Sprite2D>("Sprite2D");
		wtimer = this.GetNodeAs<Timer>("WalkAnimTimer");
		hurtbox = this.GetNodeAs<Hurtbox>("Hurtbox");
		hurtbox!.Hurt += OnHurtboxHurt;
	}
	public override void _PhysicsProcess(double delta) {
		float x = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		float y = Input.GetActionStrength("down") - Input.GetActionStrength("up");

		Velocity = new Vector2(x, y).Normalized() * Speed;

		if (x != 0) { sprite!.FlipH = x < 0; }

		sprite!.AnimOnTimer(wtimer!, Velocity != Vector2.Zero);

		MoveAndSlide();
	}
	private void OnHurtboxHurt(int damage) {
		Hp -= damage;
		if (Hp < 0) {
			//TODO
		}
	}
}

