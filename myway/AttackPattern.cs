using Abbaye.misc;
using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.myway {
    public class AttackPattern {

        public static readonly int Size = 25;
        public static readonly int Length = 25 * 25;

        private readonly CenteredArray2D board = new(Size, Size);


        private readonly int[] indexcache = new int[Size * Size];

        public AttackPattern() {
            ForEachInSpiral((x, y, it, val) => {
                int index = board.GetIndex(x, y);
                indexcache[it] = index;

                var (x2, y2) = board.GetXYFromIndex(index);
                //GD.Print($"spi: {x},{y}");
            });
        }


        public (int x, int y, int val) GetNth(int iter) {
            int index = indexcache[iter];
            var (x, y) = board.GetXYFromIndex(index);
            return (x, y, board.GetFromIndex(index));
        }


        //iterate over the board while spiraling
        public void ForEachInSpiral(Action<int, int, int, int> action) {



            int x = 0, y = 0;
            int dx = 0;
            int dy = -1;
            double max = Size * Size;
            for (int it = 0; it < max; it++) {
                action(x, y, it, board[x, y]);
                if (x == y || (x < 0 && x == -y) || (x > 0 && x == 1 - y)) {
                    int tmp = dx;
                    dx = -dy;
                    dy = tmp;
                }
                x += dx;
                y += dy;
            }
        }


        public void SetPatternAt(int x, int y, int val) {
            this.board[x, y] = val;
        }




    }
}
