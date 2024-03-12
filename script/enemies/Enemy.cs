using Abbaye.script.misc;
using Godot;
using System;
using System.Runtime.CompilerServices;



namespace Abbaye.script;
public partial class Enemy : CharacterBody2D {
    [Export]
    public float speed = 20f;
    [Export]
    public int hp = 10;
    [Export]
    public int damage = 5;

    [Export]
    public int xp_total = 1;
    [Export]
    public int orb_amount = 1;
    [Export]
    public int orb_drop_range = 5;

    [Export]
    public float anim_duration = 0.4f;
    [Export]
    public int anim_len = 2;
    [Export]
    public bool right_looking = false;

    public static readonly PackedScene xporbscene = GD.Load<PackedScene>("res://scenes/objects/xp_orb.tscn");

    Player? player;
    Sprite2D? sprite;
    //Timer? wtimer; //walk anim timer
    Hurtbox? hurtbox;
    Hitbox? hitbox;
    Node2D? lootholder;
    AnimationPlayer? animplayer;
    CollisionShape2D? colishape;
    Tween? tween;


    private bool disabled = false;

    public override void _Ready() {
        player = this.GetFirstNodeInGroupAs<Player>("player");
        sprite = GetNode<Sprite2D>("Sprite2D");
        //wtimer = GetNode<Timer>("WalkAnimTimer");
        hurtbox = GetNode<Hurtbox>("Hurtbox");
        hitbox = GetNode<Hitbox>("Hitbox");
        colishape = GetNode<CollisionShape2D>("CollisionShape2D");
        lootholder = this.GetFirstNodeInGroupAs<Node2D>("loot");
        animplayer = GetNode<AnimationPlayer>("AnimationPlayer");

        //wtimer.WaitTime = anim_wait;
        hitbox.Damage = damage;
        hurtbox.Hurt = OnHurtboxHurt;

        tween = CreateTween();
        tween.TweenProperty(sprite, "frame", anim_len - 1, anim_duration).SetTrans(Tween.TransitionType.Linear);
        tween.Finished += () => {
            sprite.Frame = 0;
            tween.Stop();
            tween.Play();
        };
        tween.Play();
    }

    public override void _PhysicsProcess(double delta) {
        if (disabled) {
            return;
        }
        Vector2 dir = GlobalPosition.DirectionTo(player!.GlobalPosition);
        Velocity = dir * speed;

        sprite!.FlipH = (dir.X < 0) ^ right_looking;
        //sprite!.AnimOnTimer(wtimer!, true);

        MoveAndSlide();
    }

    public void OnHurtboxHurt(int damage) {
        hp -= damage;
        if (hp < 0) {

            disabled = true;
            CallDeferred("DropLoot");
            animplayer!.Play("Death");

            hurtbox!.QueueFree();
            hitbox!.QueueFree();
            colishape!.SetDeferred("disabled", true);

            //QueueFree();
        } else {
            animplayer!.Play("Hit");
        }
    }

    public void DropLoot() {
        for (int iter = 0; iter < orb_amount; iter++) {
            XpOrb xporb = xporbscene.Instantiate<XpOrb>();
            Vector2 rvec = new(GD.RandRange(-10, 10), GD.RandRange(-10, 10));
            xporb.GlobalPosition = GlobalPosition + rvec;
            xporb.xp = xp_total / orb_amount;
            lootholder!.AddChild(xporb);
        }
    }
}






