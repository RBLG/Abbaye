using Abbaye.misc;
using Godot;
using System;

public partial class Hitbox : Area2D {
	[Export]
	public int damage = 1;

	public CollisionShape2D? collishape;
	public Timer? timer;

	public override void _Ready() {
		collishape = this.GetNodeAs<CollisionShape2D>("CollisionShape2D");
		timer = this.GetNodeAs<Timer>("Timer");
	   
		timer.Timeout += OnTimerTimeout;
	}


	public void TempDisable() {
		collishape!.SetDeferred("disabled", true);
		timer!.Start();
	}

	private void OnTimerTimeout() {
		collishape!.SetDeferred("disabled", false);
	}
}
