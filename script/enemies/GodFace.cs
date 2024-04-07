using Abbaye.script;
using Godot;
using System;

public partial class GodFace : Enemy {


    protected override void OnDeath() {
        MainTitle.killedgod = true;
    }
}
