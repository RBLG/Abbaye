using Abbaye.script.misc;
using Abbaye.script.content;
using Godot;
using System;

namespace Abbaye.script;
public partial class Hitbox : Area2D, IAttack {

    public CollisionShape2D? collishape;
    public Timer? timer;

    [Export]
    public int Damage { get; set; } = 1;

    public override void _Ready() {
        collishape = this.GetNodeAs<CollisionShape2D>("CollisionShape2D");
        timer = this.GetNodeAs<Timer>("Timer");

        timer.Timeout += OnTimerTimeout;
    }

    private void OnTimerTimeout() {
        collishape!.SetDeferred("disabled", false);
    }

    public void TemporaryDisable() {
        collishape!.SetDeferred("disabled", true);
        timer!.Start();
    }

    public void ConfirmHit() {
        EmitSignal(SignalName.OnHit);
    }


    [Signal]
    public delegate void OnHitEventHandler();
}
