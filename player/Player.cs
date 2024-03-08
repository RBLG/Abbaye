using Abbaye.misc;
using Abbaye.myway;
using Godot;
using System;
using System.Collections.Generic;

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

    public AttackPattern pattern = new();


    PackedScene defbullet = GD.Load<PackedScene>("res://Bullets/default_bullet.tscn");

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
        //bsholder = this.GetFirstNodeInGroupAs<Node2D>("BulletsHolder");
        Area2D dragarea = GetNode<Area2D>("XpDragArea");
        Area2D collectarea = GetNode<Area2D>("XpCollectArea");

        hurtbox.Hurt = OnHurtboxHurt;
        hurtimer.Timeout += UpdateCharWellbeing;
        dragarea.AreaEntered += OnXpDragAreaEntered;
        collectarea.AreaEntered += OnXpCollectAreaEntered;

        this.pattern.SetPatternAt(0, 1, 1);
        this.pattern.SetPatternAt(1, 0, 1);
        this.pattern.SetPatternAt(-1, -1, 1);
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
        //GD.Print($"Dmg: {damage} on {Hp}");
        Hp -= damage;
        charsprite!.UpdateAlpha(0);
        hurtimer!.Start();
        snd_hurt!.Play();
        Damagable = false;
        SetDeferred("Damagable",true);
        //UpdateCharWellbeing();
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
            var (x, y, val) = pattern.GetNth(wave.nth);
            Vector2 vec = new(x, y);
            vec = vec.Normalized();
            if (val != 0) {//val != 0
                DefaultBullet bullet = defbullet.Instantiate<DefaultBullet>();
                bullet.GlobalPosition = this.GlobalPosition + vec * 5;
                bullet.dir = vec.Rotated(wave.angle);
                AddChild(bullet);
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
        GD.Print("Levelup");
        //TODO
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

