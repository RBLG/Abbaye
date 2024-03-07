using Abbaye.misc;
using Godot;
using System;
using System.Runtime.CompilerServices;



public partial class EnemySmall1 : CharacterBody2D {
    [Export]
    public float Speed = 20f;
    [Export]
    public int Hp = 10;

    public static PackedScene xporbscene = GD.Load<PackedScene>("res://Objects/xp_orb.tscn");

    Player? player;
    Sprite2D? sprite;
    Timer? wtimer; //walk anim timer
    Hurtbox? hurtbox;
    Node2D? lootholder;

    public override void _Ready() {
        player = this.GetFirstNodeInGroupAs<Player>("player");
        sprite = GetNode<Sprite2D>("Sprite2D");
        wtimer = GetNode<Timer>("WalkAnimTimer");
        hurtbox = this.GetNodeAs<Hurtbox>("Hurtbox");
        lootholder = this.GetFirstNodeInGroupAs<Node2D>("loot");

        hurtbox.Hurt += OnHurtboxHurt;
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
            CallDeferred("DefferedLoot");
            QueueFree();
        }
    }

    public void DefferedLoot() {
        XpOrb xporb = xporbscene.Instantiate<XpOrb>();
        xporb.GlobalPosition = GlobalPosition;
        lootholder!.AddChild(xporb);
    }
}






