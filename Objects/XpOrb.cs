using Abbaye.misc;
using Godot;
using System;

public partial class XpOrb : Area2D {

    public int xp = 1;
    [Export]
    float speed = -0.5f;
    bool dragged = false;

    Player? target;
    Tween? tween;
    Sprite2D? sprite;
    CollisionShape2D? colshape;
    AnimationPlayer? animplayer;


    public override void _Ready() {
        target = this.GetFirstNodeInGroupAs<Player>("player");
        sprite = GetNode<Sprite2D>("Sprite2D");
        colshape = GetNode<CollisionShape2D>("CollisionShape2D");
        animplayer = GetNode<AnimationPlayer>("AnimationPlayer");

        tween = CreateTween();
        tween.TweenProperty(sprite, "frame", 5, 3).SetTrans(Tween.TransitionType.Linear);
        tween.Finished += OnAnimFinished;
        tween.Play();
    }

    private void OnAnimFinished() {
        sprite!.Frame = 0;
        tween!.Stop();
        tween!.Play();
    }

    public override void _PhysicsProcess(double delta) {
        if (dragged) {
            GlobalPosition = GlobalPosition.MoveToward(target!.GlobalPosition, speed);
            speed += (float)(2 * delta);
        }
    }

    public int Collect() {
        colshape!.SetDeferred("monitoring", true);
        sprite!.Visible = false;
        animplayer!.Play("XpCollected");
        return xp;
    }


    public void StartBeingDragged() {
        dragged = true;
    }

}
