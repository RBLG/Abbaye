using Abbaye.script;
using Godot;
using System;
using System.Threading.Tasks.Sources;
using static System.Formats.Asn1.AsnWriter;

namespace Abbaye.script;
public partial class DeathScreen : Control {
    CustomButton? btn_menu;
    AudioStreamPlayer2D? snd_lose;

    Tween? tween;

    public long score = 0;

    public static readonly PackedScene menu = GD.Load<PackedScene>("res://scenes/gui/main_title.tscn");

    public override void _Ready() {
        btn_menu = GetNode<CustomButton>("btn_menu");
        snd_lose = GetNode<AudioStreamPlayer2D>("snd_lose");


        btn_menu.OnClick = OnClick;

        snd_lose.Play();

        btn_menu.Disabled = true;
        Modulate = new Color(1, 1, 1, 0);
        tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 1), 2).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In);
        tween.Finished += OnFadeOutFinished;
        tween.Play();
    }

    public void OnFadeOutFinished() {
        btn_menu!.Disabled = false;
        Modulate = new Color(1, 1, 1, 1);

    }

    private void OnClick() {
        MainTitle.highscore = Math.Max(MainTitle.highscore, score);
        GetTree().ChangeSceneToPacked(menu);
    }


    public void SetScores(long nscore, double ntime, double nmaxmult) {
        score = nscore;

        int min = (int)(ntime / 60);
        int sec = (int)(ntime % 60);
        string mmult = nmaxmult.ToString("0.##");

        string label1 = $"Time survived: \t{min}min{sec}\r\nMax multiplier: x{mmult}";
        string label2 = $"Score: \t{nscore}";


        Label tmlabel = GetNode<Label>("TMLabel");
        Label scorelabel = GetNode<Label>("ScoreLabel");
        tmlabel.Text = label1;
        scorelabel.Text = label2;
    }
}
