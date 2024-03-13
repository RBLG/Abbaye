using Abbaye.script;
using Godot;
using System;

namespace Abbaye.script;
public partial class DeathScreen : Control {
    CustomButton? btn_menu;
    AudioStreamPlayer2D? snd_lose;

    Tween? tween;

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
        GetTree().Paused = false;
        GetTree().ChangeSceneToPacked(menu);
    }
}
