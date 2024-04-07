using Godot;
using System;

namespace Abbaye.script;
public partial class XpFairy : Enemy {

    public override void _Ready() {
        base._Ready();
        Timer dtimer = GetNode<Timer>("DeathTimer");
        dtimer.WaitTime = 6 + GD.RandRange(0, 2d);
        dtimer.Timeout += OnLifeTimeout;
        dtimer.Start();
    }

    public void OnLifeTimeout() {

        OnHurtboxHurt(9999);
    }
}
