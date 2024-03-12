using Abbaye.script.misc;
using Abbaye.script.content;
using Godot;
using System;
using System.Collections.Generic;


namespace Abbaye.script;
public partial class Player : CharacterBody2D {
    [Export]
    public float Speed = 70f;
    public int HpMax = 100;
    public int Hp = 100;
    public Sprite2D? charsprite;
    public Sprite2D? charspriteoff;
    public Timer? wtimer; //walk anim timer
    public Hurtbox? hurtbox;
    public Timer? hurtimer;
    public AudioStreamPlayer2D? snd_hurt;
    public Node2D? bulletroot;
    public LevelUpMenu? lvlupmenu;

    public AttackPattern pattern = new();


    public static readonly PackedScene bullet_base = GD.Load<PackedScene>("res://scenes/bullets/default_bullet.tscn");

    public static readonly PackedScene bullet_psy_ = GD.Load<PackedScene>("res://scenes/bullets/psychic_bullet.tscn");
    public static readonly PackedScene bullet_dark = GD.Load<PackedScene>("res://scenes/bullets/dark_beam.tscn");
    public static readonly PackedScene bullet_fire = GD.Load<PackedScene>("res://scenes/bullets/fireball.tscn");


    //Timer? astimer;
    //Node2D? bsholder;

    //float db_as = 1.5f;

    private readonly LevelSystem lvlsys = new();


    public override void _Ready() {
        charsprite = GetNode<Sprite2D>("CharSprite");
        charspriteoff = GetNode<Sprite2D>("CharSpriteOff");
        wtimer = GetNode<Timer>("WalkAnimTimer");
        hurtbox = GetNode<Hurtbox>("Hurtbox");
        hurtimer = GetNode<Timer>("HurtGreyTimer");
        snd_hurt = GetNode<AudioStreamPlayer2D>("snd_hurt");
        Timer astimer = GetNode<Timer>("%BulletTimer");
        Timer aswtimer = GetNode<Timer>("%BulletWaveTimer");
        Area2D dragarea = GetNode<Area2D>("XpDragArea");
        Area2D collectarea = GetNode<Area2D>("XpCollectArea");
        bulletroot = this.GetFirstNodeInGroupAs<Node2D>("Bullets");
        lvlupmenu = GetNode<LevelUpMenu>("%LevelUpMenu");

        hurtbox.Hurt = OnHurtboxHurt;
        hurtimer.Timeout += UpdateCharWellbeing;
        dragarea.AreaEntered += OnXpDragAreaEntered;
        collectarea.AreaEntered += OnXpCollectAreaEntered;

        astimer.Timeout += NextOnPattern;
        aswtimer.Timeout += NewAttackWave;

        this.OnReadyXpCrystalSettup();
    }



    public override void _PhysicsProcess(double delta) {
        float x = Input.GetActionStrength("right") - Input.GetActionStrength("left");
        float y = Input.GetActionStrength("down") - Input.GetActionStrength("up");

        Velocity = new Vector2(x, y).Normalized() * Speed;

        if (x != 0) { charsprite!.FlipH = x < 0; }
        if (x != 0) { charspriteoff!.FlipH = x < 0; }
        if (wtimer!.IsStopped()) {
            charsprite!.Animate();
            charspriteoff!.Animate();
            wtimer.Start();
        }

        MoveAndSlide();
    }

    public bool Damagable = true;
    private void OnHurtboxHurt(int damage) {
        if (!Damagable) {
            return;
        }
        Hp -= damage;
        charsprite!.UpdateAlpha(0);
        hurtimer!.Start();
        snd_hurt!.Play();
        Damagable = false;
        SetDeferred("Damagable", true); // HACK because it would proc twice/four time per actual collision
        if (Hp < 0) {
            //TODO
        }
    }

    public void UpdateCharWellbeing() {
        charsprite!.UpdateAlpha((float)Hp / HpMax);
    }

    //////////////////////////////// ATTACK ///////////////////////////////
    #region attack
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
            var (x, y, val) = pattern.GetXYValueFromNth(wave.nth);
            if (val != AttackPattern.BULLET_NONE) {//val != 0
                Vector2 vec = new(x, y);
                vec = vec.Rotated(wave.angle);
                vec = vec.Normalized();
                Vector2 pos = vec;
                DefaultBullet bullet;
                if (val == AttackPattern.BULLET_PSY) {
                    bullet = bullet_psy_.Instantiate<DefaultBullet>();
                    pos *= 2;
                } else if (val == AttackPattern.BULLET_DARK) {
                    bullet = bullet_dark.Instantiate<DefaultBullet>();
                    pos *= 4;

                } else if (val == AttackPattern.BULLET_FIRE) {
                    bullet = bullet_fire.Instantiate<DefaultBullet>();
                    pos *= 8;

                } else { //default, just in case but shouldnt be needed;
                    bullet = bullet_base.Instantiate<DefaultBullet>();
                    pos *= 4;
                }
                bullet.dir = vec;
                bullet.UpdateVisualRotation(vec.Angle());
                if (val == AttackPattern.BULLET_DARK) {
                    bullet.GlobalPosition = pos;
                    AddChild(bullet);
                } else {
                    bullet.GlobalPosition = GlobalPosition + pos;
                    bulletroot!.AddChild(bullet);
                }
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
    #endregion attack

    /////////////////////////////// CRYSTAL SHAKE /////////////////////////
    #region xp
    Tween? crystalshaker;
    Sprite2D? crystalsprite;
    Sprite2D? crystalgreysprite;
    private void OnReadyXpCrystalSettup() {
        crystalsprite = GetNode<Sprite2D>("CrystalSprite");
        crystalgreysprite = GetNode<Sprite2D>("%OffCrystalSprite");

        lvlsys.OnLevelUp = OnLevelUp;
        ResetShaker();
    }

    private void OnXpDragAreaEntered(Area2D area) {
        if (area is XpOrb xporb) {
            xporb.StartBeingDragged();
        }
    }

    private void OnXpCollectAreaEntered(Area2D area) {
        if (area is XpOrb xporb) {
            int xp = xporb.Collect();
            lvlsys.AddXp(xp);
            DoTheCrystalShake();
            UpdateLevelShine();
        }
    }

    public void OnLevelUp(int level) {
        lvlupmenu!.OnLevelUp(pattern, level);
    }

    private void ResetShaker() {
        crystalshaker = CreateTween();
        crystalshaker.TweenProperty(crystalsprite, "scale", new Vector2(1, 1), 1).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.Out);
    }

    private void DoTheCrystalShake() {
        crystalsprite!.Scale += new Vector2(0.15f, 0.15f);
        crystalshaker!.Stop();
        if (!crystalshaker!.IsValid()) {
            //crystalshaker.Unreference(); //maybe?
            ResetShaker();
        }
        crystalshaker!.Play();
    }

    public void UpdateLevelShine() {
        Color color = crystalgreysprite!.SelfModulate;
        color.A = 1 - lvlsys.GetCompletionRatio();
        crystalgreysprite!.SelfModulate = color;
    }
    #endregion xp
}

