using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.misc {
    public class CenteredArray2D {

        protected int[] board;
        protected int width;
        protected int height;
        protected int miwi;
        protected int mihe;

        public CenteredArray2D(int nwidth, int nheight) {
            width = nwidth;
            height = nheight;
            board = new int[width * height];
            miwi = width / 2;
            mihe = height / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetIndex(int x, int y) => x + miwi + (y + mihe) * width;

        public int this[int x, int y] {
            get => board[GetIndex(x, y)];
            set => board[GetIndex(x, y)] = value;
        }

        public int GetFromIndex(int index) => board[index];

        public (int x, int y) GetXYFromIndex(int index) {
            int x = index % width;
            int y = index / width;
            return (x - miwi, y - mihe);
        }

        public void SetByIndex(int index, int val) {
            board[index] = val;
        }

    }
}
