using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class EnemySmall1 : CharacterBody2D
{
    [Export]
    public float movement_speed = 20;

    public CharacterBody2D player;

#pragma warning disable CS8618 //nullable
    public EnemySmall1() {
        
    }
#pragma warning restore CS8618 

    public override void _Ready() {
        player = (CharacterBody2D)GetTree().GetFirstNodeInGroup("player");
    }

    public override void _PhysicsProcess(double _delta) {
        Vector2 dir = GlobalPosition.DirectionTo(player.GlobalPosition);
        Velocity = dir * movement_speed;
        MoveAndSlide();
    }

    class EnemySmall1Inner
    {
        public CharacterBody2D player;

        public EnemySmall1Inner(CharacterBody2D nplayer) {
            player = nplayer;
        }
    }
}




