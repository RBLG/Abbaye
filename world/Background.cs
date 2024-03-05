using Abbaye.misc;
using Godot;
using System;

public partial class Background : TileMap {

	//public Area2D? camboundary;

	protected int TileSize = 40;

	protected RollingArray2D? tileboard;

	//Window? root;
	Player? player;
	Vector2 vpsize;

	public override void _Ready() {
		//root = GetTree().Root;
		player = this.GetFirstNodeInGroupAs<Player>("player");

		vpsize = GetViewportRect().Size;
		int nwidth_ = (int)(vpsize.X / TileSize + 4);
		int nheight = (int)(vpsize.Y / TileSize + 4);
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
	int[] badlands = new int[] { 1, 2, 3, 4, 4, 4 };
	//Noise noise = new FastNoiseLite();

	private int GenerateTile(RollingArray2D arr, int x, int y) {
		Vector2 pos = new((x - vpsize.X / 2 / TileSize) * 0.6f, y - vpsize.Y / 2 / TileSize);
		int nval = (int)(pos.Length() + GD.RandRange(0, 0.7));
		nval = nval switch {
			< 7 => 0,
			< 30 => 1,
			< 60 => 2,
			< 65 => 3,
			< 85 => 4,
			< 90 => 3,
			_ => badlands[rand.Next(badlands.Length)]
		}; ;

		//int nval = rand.Next(6);
		//int nval = ((x + y) / 4) % 5;
		//nval = (nval < 0) ? nval + 5 : nval;
		return nval;
	}

	protected Vector2I GetTruePos(int x, int y) {
		return new Vector2I(x - tileboard!.Width, y - tileboard.Height);

	}

	private void SetTile(RollingArray2D arr, int x, int y, int value) {
		bool sleft = arr[x - 1, y] == value;
		bool srigh = arr[x + 1, y] == value;
		bool sup__ = arr[x, y - 1] == value;
		bool sdown = arr[x, y + 1] == value;

		int tx = x * 2 - tileboard!.Width;
		int ty = y * 2 - tileboard.Height;

		int vpos = value * 2;

		//set the 2x2 tile with the 4 border variants
		SetCell(0, new(tx + 0, ty + 0), 0, LeUp(vpos, GetBorderVariant(sleft, sup__)));
		SetCell(0, new(tx + 1, ty + 0), 0, RiUp(vpos, GetBorderVariant(srigh, sup__)));
		SetCell(0, new(tx + 0, ty + 1), 0, LeDo(vpos, GetBorderVariant(sleft, sdown)));
		SetCell(0, new(tx + 1, ty + 1), 0, RiDo(vpos, GetBorderVariant(srigh, sdown)));

		bool sleup = arr[x - 1, y - 1] == value;
		bool sriup = arr[x + 1, y - 1] == value;
		bool sledo = arr[x - 1, y + 1] == value;
		bool srido = arr[x + 1, y + 1] == value;

		//updating sides
		if (sleft) {
			SetCell(0, new(tx - 1, ty + 0), 0, RiUp(vpos, GetBorderVariant(true, sleup)));
			SetCell(0, new(tx - 1, ty + 1), 0, RiDo(vpos, GetBorderVariant(true, sledo)));
		}
		if (sup__) {
			SetCell(0, new(tx + 0, ty - 1), 0, LeDo(vpos, GetBorderVariant(sleup, true)));
			SetCell(0, new(tx + 1, ty - 1), 0, RiDo(vpos, GetBorderVariant(sriup, true)));
		}
		if (srigh) {
			SetCell(0, new(tx + 2, ty + 0), 0, LeUp(vpos, GetBorderVariant(true, sriup)));
			SetCell(0, new(tx + 2, ty + 1), 0, LeDo(vpos, GetBorderVariant(true, srido)));
		}
		if (sdown) {
			SetCell(0, new(tx + 0, ty + 2), 0, LeUp(vpos, GetBorderVariant(sledo, true)));
			SetCell(0, new(tx + 1, ty + 2), 0, RiUp(vpos, GetBorderVariant(srido, true)));
		}


	}


	// variants: xy=0; x_=1; _y=2; __=3
	private static int GetBorderVariant(bool x, bool y) => x ? (y ? 0 : 2) : (y ? 4 : 6); //*2 cuz 1 tile is 2x2

	private static Vector2I LeUp(int x, int y) => new(x + 0, y + 0); //add the tile quadrant offset
	private static Vector2I RiUp(int x, int y) => new(x + 1, y + 0);
	private static Vector2I LeDo(int x, int y) => new(x + 0, y + 1);
	private static Vector2I RiDo(int x, int y) => new(x + 1, y + 1);

	private void EraseTile(int x, int y) {

		x = x * 2 - tileboard!.Width;
		y = y * 2 - tileboard.Height;
		EraseCell(0, LeUp(x, y));
		EraseCell(0, RiUp(x, y));
		EraseCell(0, LeDo(x, y));
		EraseCell(0, RiDo(x, y));
		//EraseCell(1, LeUp(x, y));
		//EraseCell(1, RiUp(x, y));
		//EraseCell(1, LeDo(x, y));
		//EraseCell(1, RiDo(x, y));
	}

}


