using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.script.misc
{

    /// <summary>
    /// array that hold what tiles are visible in scene.
    /// offset is the true position of index 0,0
    /// </summary>
    public class RollingArray2D
    {

        protected int[] board;
        protected int width;
        protected int height;
        protected int offsetx;
        protected int offsety;

        public int Width { get => width; }
        public int Height { get => height; }

        public Func<RollingArray2D, int, int, int> OnRoll { get; set; } = (arr, x, y) => -1;
        public Action<RollingArray2D, int, int, int> OnSet { get; set; } = (arr, x, y, val) => { };
        public Action<int, int> OnDelete { get; set; } = (x, y) => { };

        public int OffsetX
        {
            get => offsetx; set
            {
                if (value != offsetx) { PrepareNewColumns(value); }
                offsetx = value;
            }
        }
        public int OffsetY
        {
            get => offsety; set
            {
                if (value != offsety) { PrepareNewRows(value); }
                offsety = value;
            }
        }

        public RollingArray2D(int nwidth, int nheight, int noffsetx, int noffsety, Func<RollingArray2D, int, int, int> nroll)
        {
            width = nwidth;
            height = nheight;
            offsetx = noffsetx;
            offsety = noffsety;
            board = new int[width * height];
            Fill();
            OnRoll = nroll;
        }

        public RollingArray2D(int nwidth, int nheight, RollingArray2D old) : this(nwidth, nheight, old.offsetx, old.offsety, old.OnRoll)
        {
            // TODO on resize, generates tiles around the old (which is centered)
        }

        //non rolled: -inf to +inf
        //rolled: 0 to size
        //index: 1d, 0 to size

        protected int GetIndex(int x, int y)
        {
            //loop the coords
            x %= width;
            y %= height;
            //correcting remainder into modulus
            x = x < 0 ? x + width : x;
            y = y < 0 ? y + height : y;

            //flatten the coords
            return x + y * width;
        }

        public int this[int x, int y]
        {
            get => board[GetIndex(x, y)];
            set => board[GetIndex(x, y)] = value;
        }

        public void Fill()
        {
            for (int itx = offsetx; itx < offsetx + width; itx++)
            {
                for (int ity = offsety; ity < offsety + height; ity++)
                {
                    int val = OnRoll(this, itx, ity);
                    this[itx, ity] = val;
                    OnSet(this, itx, ity, val);
                }
            }
        }



        public void PrepareNewColumns(int noffsetx)
        {
            int start = Math.Min(offsetx, noffsetx);
            int end__ = Math.Max(offsetx, noffsetx);

            //the fact that its always incremential mean it could cause artefacts if preparing a tile
            //is aware of it neighbors.
            //but wont happen unless moving too fast
            for (int itx = start; itx < end__; itx++)
            {
                for (int ity = offsety; ity < offsety + height; ity++)
                {
                    OnDelete(itx, ity);
                    int nitx = itx + (noffsetx < offsetx ? 0 : width);
                    int val = OnRoll(this, nitx, ity);
                    this[itx, ity] = val;
                    OnSet(this, nitx, ity, val);
                }
            }
        }


        public void PrepareNewRows(int noffsety)
        {
            int start = Math.Min(offsety, noffsety);
            int end__ = Math.Max(offsety, noffsety);

            for (int ity = start; ity < end__; ity++)
            {
                for (int itx = offsetx; itx < offsetx + width; itx++)
                {
                    OnDelete(itx, ity);
                    int nity = ity + (noffsety < offsety ? 0 : height);
                    int val = OnRoll(this, itx, nity);
                    this[itx, ity] = val;
                    OnSet(this, itx, nity, val);
                }
            }
        }

    }
}
