using Godot;
using System;

namespace Abbaye.script;
public partial class SocketMap : TileMap {



    public override void _Input(InputEvent evnt) {
        if (evnt.IsAction("click")) {
            OnClick?.Invoke();
        }
    }

    public delegate void OnClickEventHandler();
    public OnClickEventHandler? OnClick { get; set; }

}
