using Godot;
using System;

namespace Abbaye.script;
public partial class XpFairy : Enemy {

    Timer? dtimer;

    public override void _Ready() {
        base._Ready();
        dtimer = GetNode<Timer>("DeathTimer");
        dtimer.WaitTime += GD.RandRange(0, 1d);
        dtimer.Timeout += OnLifeTimeout;
    }

    public void OnLifeTimeout() {

        OnHurtboxHurt(9999);
    }
}
