using Abbaye.script.misc;
using Godot;
using System;
using System.Runtime.CompilerServices;



namespace Abbaye.script;
public partial class Enemy : CharacterBody2D {
    [Export]
    public float speed = 20f;
    [Export]
    public float max_speed_variation = 0.2f;

    [Export]
    public float max_dir_variation = 0f;

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
    public int score_value = 1;

    [Export]
    public float anim_duration = 0.4f;
    [Export]
    public int anim_len = 2;
    [Export]
    public bool right_looking = false;
    [Export]
    public int run_radius_malus = 0;

    public static readonly PackedScene xporbscene = GD.Load<PackedScene>("res://scenes/objects/xp_orb.tscn");

    float speedvar = 1;
    float dirvar = 1;
    int runvar = 0;

    Player? player;
    Sprite2D? sprite;
    //Timer? wtimer; //walk anim timer
    Hurtbox? hurtbox;
    Hitbox? hitbox;
    Node2D? lootholder;
    AnimationPlayer? deathanim;
    AnimationPlayer? hitanim;
    CollisionShape2D? colishape;
    Tween? tween;
    AudioStreamPlayer2D? snd_death;

    private bool disabled = false;

    public override void _Ready() {
        player = this.GetFirstNodeInGroupAs<Player>("player");
        sprite = GetNode<Sprite2D>("Sprite2D");
        //wtimer = GetNode<Timer>("WalkAnimTimer");
        hurtbox = GetNode<Hurtbox>("Hurtbox");
        hitbox = GetNode<Hitbox>("Hitbox");
        colishape = GetNode<CollisionShape2D>("CollisionShape2D");
        lootholder = this.GetFirstNodeInGroupAs<Node2D>("loot");
        deathanim = GetNode<AnimationPlayer>("DeathAnimation");
        hitanim = GetNode<AnimationPlayer>("HitAnimation");
        snd_death = GetNode<AudioStreamPlayer2D>("snd_death");

        dirvar = (float)GD.RandRange(-max_dir_variation, max_dir_variation) * 2 * MathF.PI;
        speedvar = (float)GD.RandRange(1 - max_speed_variation, 1 + max_speed_variation);
        this.anim_duration *= speedvar;
        runvar = GD.RandRange(0, 50) + GD.RandRange(0, 50);

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
        if (disabled) { return; }
        Vector2 this2targ = player!.GlobalPosition - GlobalPosition;                // get the vector from this to the player
        Vector2 dir = this2targ.Normalized();

        this2targ.X = Mathf.Sign(this2targ.X) * run_radius_malus * -0.5f;
        if (0 < this2targ.Y) {
            this2targ.Y -= run_radius_malus;
        }

        this2targ.X *= 0.5625f;                                                     // scale X so that XY ratio is roughly screen ratio
        float dist = Mathf.Max(Mathf.Abs(this2targ.X), Math.Abs(this2targ.Y));      // getting the highest absolute value between X and Y
        dist = Mathf.Max(0, dist - 300 - runvar);                                   // set the minimum distance from player with random margin

        float runratio = dist * 0.02f;                                              // get the ratio for each speed values
        float walkratio = Mathf.Max(0, 1 - runratio);                               // |
        float realspeed = speed * walkratio + player.Speed * runratio;              // interpolation between defined speed and player speed (aka camera speed)

        dir = dir.Rotated(dirvar * walkratio);
        //Velocity = dir * realspeed;
        //MoveAndSlide();
        var movec = dir * realspeed;
        movec.X = (float)(movec.X * delta);
        movec.Y = (float)(movec.Y * delta);
        MoveAndCollide(movec);

        sprite!.FlipH = (dir.X < 0) ^ right_looking;
    }


    public bool dmgable = true;

    public void SetDmgable() {
        dmgable = true;
    }

    public static bool snd_enabled = true;

    public void EnforceSndOn() {
        snd_enabled = true;
    }

    public void OnHurtboxHurt(int damage) {
        if (!dmgable) {
            return;
        }
        dmgable = false;
        CallDeferred("SetDmgable");

        hp -= damage;
        if (hp <= 0) {
            disabled = true;
            dead = true;
            if (snd_enabled) {
                snd_death!.Play();
                snd_enabled = false;
            }
            SetDeferred("snd_enabled", true);
            CallDeferred("DropLoot");
            deathanim!.Play("Death");
            player!.ReceiveScore(score_value);

            hurtbox!.QueueFree();
            hitbox!.QueueFree();
            colishape!.SetDeferred("disabled", true);
            OnDeath();
            //QueueFree();
        } else {
            hitanim!.Play("Hit");
        }
    }

    protected virtual void OnDeath() {

    }

    public void DropLoot() {
        for (int iter = 0; iter < orb_amount; iter++) {
            XpOrb xporb = xporbscene.Instantiate<XpOrb>();
            Vector2 rvec = new((float)GD.RandRange(-1d, 1d), (float)GD.RandRange(-1d, 1d));
            rvec *= orb_drop_range;
            xporb.GlobalPosition = GlobalPosition + rvec;
            xporb.SetXpAmount(xp_total / orb_amount);
            lootholder!.AddChild(xporb);
        }
    }

    public bool dead = false;

    public void DieIfDead() {
        if (dead) {
            deathanim!.Play("Death");
        }
    }
}