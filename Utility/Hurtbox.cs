using Abbaye.misc;
using Abbaye.myway;
using Godot;
using System;

public partial class Hurtbox : Area2D {
	[Export]
	public HurtboxType type = HurtboxType.Cooldown;
	public CollisionShape2D? collishape;
	public Timer? timer;

	public override void _Ready() {
		collishape = this.GetNodeAs<CollisionShape2D>("CollisionShape2D");
		timer = this.GetNodeAs<Timer>("Timer");
		this.AreaEntered += OnAreaEntered;
		timer.Timeout += OnTimerTimeout;
	}


	public enum HurtboxType {
		Cooldown, HitOnce, DisableHitbox
	}

	private void OnAreaEntered(Area2D area) {
		if (area is IAttack att) {
			switch (type) {
				case HurtboxType.Cooldown:
					collishape!.SetDeferred("disabled", true);
					timer!.Start();
					break;
				case HurtboxType.HitOnce:
					//TODO
					break;
				case HurtboxType.DisableHitbox:
					att.TemporaryDisable();
					break;
				default: break;
			}
			int dmg = att.Damage;
			this.EmitSignal(SignalName.Hurt, dmg);
		}
	}


	private void OnTimerTimeout() {
		collishape!.SetDeferred("disabled", true);
	}

	[Signal]
	public delegate void HurtEventHandler(int damage);
}