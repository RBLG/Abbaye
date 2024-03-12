using Abbaye.script.misc;
using Godot;
using System;

namespace Abbaye.script;
public partial class DefaultBullet : Node2D {
    [Export]
    int speed = 100;
    [Export]
    int damage = 10;
    [Export]
    bool rotate = false;
    [Export]
    Mode mode = Mode.OneHit;

    public enum Mode { OneHit, Continuous }

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

    public void UpdateVisualRotation(float angle) {
        if (rotate) {
            Rotation = angle;
        }
    }

    public void OnHit() {
		if (mode is Mode.OneHit) {
			this.QueueFree();
		}
    }

    public void Timeout() {
        this.QueueFree();
    }




}
