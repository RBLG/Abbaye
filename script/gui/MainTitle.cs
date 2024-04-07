using Abbaye.script;
using Godot;
using System;


namespace Abbaye.script;

public partial class MainTitle : Control {
    CustomButton? btn_play;

    public static readonly PackedScene world = GD.Load<PackedScene>("res://scenes/world/world.tscn");

    public override void _Ready() {
        btn_play = GetNode<CustomButton>("btn_play");

        GetTree().Paused = false;
        btn_play.OnClick = OnClick;
        this.UpdateHighscore();
    }

    private void OnClick() {
        GetTree().ChangeSceneToPacked(world);
    }


    public static long highscore = -1;
    public static bool killedgod = false;

    public void UpdateHighscore() {
        if (highscore < 0) {
            return;
        }
        Label scorelabel = GetNode<Label>("ScoreLabel");
        scorelabel.Text = $"Highscore:\r\n{highscore}";
        scorelabel.Visible = true;

        TextureRect trophyon = GetNode<TextureRect>("ImgTrophyOn");
        TextureRect trophyoff = GetNode<TextureRect>("ImgTrophyOff");
        if (killedgod) {
            trophyon.Visible = true;
            trophyoff.Visible = false;
        } else {
            trophyon.Visible = false;
            trophyoff.Visible = true;
        }
    }
}
