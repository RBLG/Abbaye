using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 60.0f;

	public override void _PhysicsProcess(double delta) {
		float x = Input.GetActionStrength("right") - Input.GetActionStrength("left");
		float y = Input.GetActionStrength("down") - Input.GetActionStrength("up");

		Velocity = new Vector2(x, y).Normalized() * Speed;

	


		MoveAndSlide();
	}
}
