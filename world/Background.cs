using Abbaye.misc;
using Godot;
using System;

public partial class Background : TileMap {

	//public Area2D? camboundary;

	protected int TileSize = 40;

	protected RollingArray2D? tileboard;

	Window? root;
	Player? player;

	public override void _Ready() {
		root = GetTree().Root;
		player = this.GetFirstNodeInGroupAs<Player>("player");
		//camboundary = this.GetFirstNodeInGroupAs<Area2D>("CameraBoundary");
		//camboundary!.BodyShapeEntered += OnBodyShapeEntered;
		//camboundary.BodyShapeExited += OnBodyShapeExited;

		int nwidth_ = root.Size.X / TileSize + 4;
		int nheight = root.Size.Y / TileSize + 4;
		int noffsetx = (int)(player.GlobalPosition.X / TileSize);
		int noffsety = (int)(player.GlobalPosition.Y / TileSize);
		tileboard = new(nwidth_, nheight, noffsetx, noffsety, GenerateTile);
		tileboard.OnDelete = EraseTile;
		tileboard.OnSet = SetTile;
		this.CallDeferred("DeferredReady");
	}

	public void DeferredReady() {
		tileboard!.Fill();
		GD.Print(this.TileSet.ResourceName);
	}

	public override void _Process(double delta) {
		this.CallDeferred("UpdateTilemap");
	}

	public void UpdateTilemap() {
		Vector2 pos = player!.GlobalPosition / TileSize;

		//updating the offset also generate the new tiles
		tileboard!.OffsetX = (int)pos.X;
		tileboard!.OffsetY = (int)pos.Y;

	}

	Random rand = new();

	private int GenerateTile(RollingArray2D arr, int x, int y) {
		//int nval = rand.Next(3);
		int nval = ((x + y) / 2) % 3;
		nval = (nval < 0) ? nval + 3 : nval;
		return nval;
	}

	protected Vector2I GetTruePos(int x, int y) {
		return new Vector2I(x - tileboard!.Width / 2, y - tileboard.Height / 2);

	}

	private void SetTile(int x, int y, int value) {
		SetCell(0, GetTruePos(x, y), 0, new Vector2I(value, 0));
		//GD.Print("set cell " + x + "/" + y);

	}

	private void EraseTile(int x, int y) {
		EraseCell(0, GetTruePos(x, y));
		//GD.Print("erased cell " + x + "/" + y);
	}

}


