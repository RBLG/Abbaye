using Abbaye.script.misc;
using Abbaye.script.content;
using Godot;
using System;
using System.Collections.Generic;


namespace Abbaye.script;
public partial class Player : CharacterBody2D {

    public const float SPEED = 80f;

    public float Speed = SPEED;
    public int HpMax = 100;
    public int Hp = 100;
    public Sprite2D? charsprite;
    public Sprite2D? charspriteoff;
    public Timer? wtimer; //walk anim timer
    public Hurtbox? hurtbox;
    public Timer? hurtimer;
    public AudioStreamPlayer2D? snd_hurt;
    public AudioStreamPlayer2D? snd_cast;
    public AudioStreamPlayer2D? snd_lvl;
    public AudioStreamPlayer2D? snd_xp;
    public Node2D? bulletroot;
    public LevelUpMenu? lvlupmenu;
    public CanvasLayer? guilayer;

    public AttackPattern pattern = new();

    public double score = 0;
    public double multiplier = 1;
    public double maxmult = 1;
    public double time = 0;

    public static readonly PackedScene bullet_base = GD.Load<PackedScene>("res://scenes/bullets/default_bullet.tscn");

    public static readonly PackedScene bullet_psy_ = GD.Load<PackedScene>("res://scenes/bullets/psychic_bullet.tscn");
    public static readonly PackedScene bullet_dark = GD.Load<PackedScene>("res://scenes/bullets/dark_beam.tscn");
    public static readonly PackedScene bullet_fire = GD.Load<PackedScene>("res://scenes/bullets/fireball.tscn");

    public static readonly PackedScene death_screen = GD.Load<PackedScene>("res://scenes/gui/death_screen.tscn");


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
        snd_cast = GetNode<AudioStreamPlayer2D>("snd_cast");
        snd_lvl = GetNode<AudioStreamPlayer2D>("snd_lvl");
        Timer astimer = GetNode<Timer>("%BulletTimer");
        Timer aswtimer = GetNode<Timer>("%BulletWaveTimer");
        Area2D dragarea = GetNode<Area2D>("XpDragArea");
        Area2D collectarea = GetNode<Area2D>("XpCollectArea");
        bulletroot = this.GetFirstNodeInGroupAs<Node2D>("Bullets");
        lvlupmenu = GetNode<LevelUpMenu>("%LevelUpMenu");

        guilayer = GetNode<CanvasLayer>("GuiLayer");

        hurtbox.Hurt = OnHurtboxHurt;
        hurtimer.Timeout += UpdateCharWellbeing;
        dragarea.AreaEntered += OnXpDragAreaEntered;
        collectarea.AreaEntered += OnXpCollectAreaEntered;

        astimer.Timeout += NextOnPattern;
        astimer.Timeout += () => { time += astimer.WaitTime; };
        aswtimer.Timeout += NewAttackWave;

        this.OnReadyXpCrystalSettup();
    }

    public long x1framecount = -9999;
    public long x2framecount = -9999;
    public long y1framecount = -9999;
    public long y2framecount = -9999;

    public readonly int acclen = 3;
    public readonly float accleninv = 1 / 3f;

    public override void _PhysicsProcess(double delta) {
        float x1 = Input.GetActionStrength("right");
        float x2 = Input.GetActionStrength("left");
        float y1 = Input.GetActionStrength("down");
        float y2 = Input.GetActionStrength("up");
        if (0 < x1) { x1framecount = Math.Clamp(x1framecount + 1, +1, +acclen); } //
        else /*   */{ x1framecount = Math.Clamp(x1framecount - 1, -acclen, -1); }
        if (0 < x1) { x2framecount = Math.Clamp(x2framecount + 1, +1, +acclen); } //
        else /*   */{ x2framecount = Math.Clamp(x2framecount - 1, -acclen, -1); }
        if (0 < x1) { y1framecount = Math.Clamp(y1framecount + 1, +1, +acclen); } //
        else /*   */{ y1framecount = Math.Clamp(y1framecount - 1, -acclen, -1); }
        if (0 < x1) { y2framecount = Math.Clamp(y2framecount + 1, +1, +acclen); } //
        else /*   */{ y2framecount = Math.Clamp(y2framecount - 1, -acclen, -1); }
        x1 *= (x1framecount - 1) * accleninv;



        float x = x1 - x2;
        float y = y1 - y2;

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
        Multiplier = 1;
        Damagable = false;
        SetDeferred("Damagable", true); // HACK because it would proc twice/four time per actual collision
        if (Hp < 0) {
            OnDeath();
        }
    }

    bool dead = false;

    public void OnDeath() {
        dead = true;
        DeathScreen ds = death_screen.Instantiate<DeathScreen>();
        ds.SetScores((long)score, time, maxmult);
        GetTree().Paused = true;
        guilayer!.AddChild(ds);
    }

    public void UpdateCharWellbeing() {
        charsprite!.UpdateAlpha(((float)Hp) / HpMax);
    }

    public void ReceiveScore(int nscore) {
        Score += (int)(nscore * multiplier);
        Multiplier *= 1.001f;
    }

    public double Multiplier {
        get => multiplier; set {
            multiplier = value;
            maxmult = Mathf.Max(maxmult, multiplier);
            //TODO update gui
        }
    }

    public double Score {
        get => score; set {
            score = value;
            //TODO update gui
        }
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
                snd_cast!.Play();
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
        snd_xp = GetNode<AudioStreamPlayer2D>("snd_xp");

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
            snd_xp!.Play();
        }
    }

    public void OnLevelUp(int level) {
        if (dead) {
            return;
        }

        //snd_lvl!.Play();
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

