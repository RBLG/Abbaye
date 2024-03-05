using Abbaye.misc;
using Abbaye.myway;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Player : CharacterBody2D {
    [Export]
    public float Speed = 60f;
    [Export]
    public int Hp = 100;
    public Sprite2D? sprite;
    public Timer? wtimer; //walk anim timer
    public Hurtbox? hurtbox;

    public AttackPattern pattern = new();


    PackedScene defbullet = GD.Load<PackedScene>("res://Bullets/default_bullet.tscn");

    //Timer? astimer;
    Node2D? bsholder;

    float db_as = 1.5f;


    public override void _Ready() {
        sprite = this.GetNodeAs<Sprite2D>("Sprite2D");
        wtimer = this.GetNodeAs<Timer>("WalkAnimTimer");
        hurtbox = this.GetNodeAs<Hurtbox>("Hurtbox");
        Timer astimer = this.GetNodeAs<Timer>("%BulletTimer");
        Timer aswtimer = this.GetNodeAs<Timer>("%BulletWaveTimer");
        bsholder = this.GetFirstNodeInGroupAs<Node2D>("BulletsHolder");

        hurtbox!.Hurt += OnHurtboxHurt;

        this.pattern.SetPatternAt(0, 1, 1);
        this.pattern.SetPatternAt(1, 0, 1);
        this.pattern.SetPatternAt(-1, -1, 1);
        astimer.Timeout += NextOnPattern;
        aswtimer.Timeout += NewAttackWave;
    }

    ////////////////////////////////ATTACK//////////////////

    public class AttackWave {
        public int nth = 1;
        public float angle = (float)GD.RandRange(0, 2 * Math.PI);
    }

    public List<AttackWave> awaves = new();

    public void NewAttackWave() {
        awaves.Add(new());
    }

    public void NextOnPattern() {
        int iter = 0;
        foreach (AttackWave wave in awaves) {
            var (x, y, val) = pattern.GetNth(wave.nth);
            Vector2 vec = new(x, y);
            vec = vec.Normalized();
            if (val != 0) {//val != 0
                DefaultBullet bullet = defbullet.Instantiate<DefaultBullet>();
                bullet.GlobalPosition = this.GlobalPosition + vec * 3;
                bullet.dir = vec.Rotated(wave.angle);
                bsholder!.AddChild(bullet);
            }
            wave.nth++;
            if (AttackPattern.Length <= wave.nth) {
                wave.nth = 1;
                CallDeferred("DeferredAwaveRemove", iter);
            }
            iter++;
        }
    }

    public void DeferredAwaveRemove(int index) {
        this.awaves.RemoveAt(index); //would have caused a bug if two wave ended at once, but not the case
    }





    ////////////////////////////////ATTACK//////////////////
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

