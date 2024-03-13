using Abbaye.script;
using Godot;
using System;

public partial class MainTitle : Control {
	CustomButton? btn_play;

	public static readonly PackedScene world = GD.Load<PackedScene>("res://scenes/world/world.tscn");

	public override void _Ready() {
		btn_play = GetNode<CustomButton>("btn_play");

        GetTree().Paused = false;
        btn_play.OnClick = OnClick;
	}

	private void OnClick() {
		GetTree().ChangeSceneToPacked(world);
	}
}
