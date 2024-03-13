using Godot;
using System;

namespace Abbaye.script;

public partial class CustomButton : TextureButton {
    AnimationPlayer? anim;

    public override void _Ready() {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        ButtonDown += OnButtonClicked;
    }

    public void OnButtonClicked() {
        anim!.Play("Click");
    }

    public void OnButtonAnimEnded() {
        OnClick?.Invoke();
    }

    public delegate void OnClickEventHandler();

    public OnClickEventHandler? OnClick { get; set; }

}
