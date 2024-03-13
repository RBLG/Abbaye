using Abbaye.script;
using Abbaye.script.content;
using Godot;
using System;

namespace Abbaye.script;
public partial class DialogBox : Control {

    public override void _Ready() {
        Label textbox = GetNode<Label>("%TextBox");

        int rindex = GD.RandRange(0, Dialogs.dialogs.Length - 1);
        textbox.Text = Dialogs.dialogs[rindex];

        GetTree().Paused = true;
    }

    bool caninput = false;


    public void OnFadeInDone() {
        caninput = true;
    }

    public void Close() {
        GetTree().Paused = false;
        QueueFree();
    }

    public override void _Input(InputEvent @event) {
        if (caninput && (@event.IsPressed())) {
            Close();
        }
    }
}
