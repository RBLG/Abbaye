using Abbaye.misc;
using Godot;
using System;

public partial class BoundedCamera2D : Camera2D {

	Window? root;
	Area2D? area;
	CollisionShape2D? colli;

	public override void _Ready() {
		root = GetTree().Root;
		area = this.GetNodeAs<Area2D>("CameraBoundary");
		colli = this.GetNodeAs<CollisionShape2D>("CameraBoundary/CollisionShape2D");
		root.SizeChanged += OnSizeChanged;

		UpdateAreaSize();
	}

	private void OnSizeChanged() {
		UpdateAreaSize();
	}


	protected void UpdateAreaSize() {
		this.CallDeferred("UpdateAreaSizeDeferred");
	}
	protected void UpdateAreaSizeDeferred() {
		Vector2 ssize = root!.Size;
		RectangleShape2D? shape = this.colli!.Shape as RectangleShape2D;
		shape!.Size = new Vector2(1, 1);
		area!.Scale = ssize; // + new Vector2(10,10);
		GD.Print(shape.Size);
		Camera2D cam = new();
	}
}
