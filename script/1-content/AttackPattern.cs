using Abbaye.script.misc;
using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abbaye.script.content;

public class AttackPattern
{

    public static readonly int Size = 15;
    public static readonly int Length = 15 * 15;


    // pool (toxic)
    // beam (dark)
    // slow ball (fire)
    // close range swing (psy)

    public const int BULLET_ENEMY = -1;

    public const int BULLET_NONE = 0;
    public const int BULLET_PSY = 1;
    public const int BULLET_DARK = 2;
    public const int BULLET_FIRE = 3;
    public const int BULLET_TOXIC = 4;



    public readonly CenteredArray2D board = new(Size, Size);


    private readonly int[] indexcache = new int[Size * Size];
    private readonly int[] nthcache = new int[Size * Size];

    public AttackPattern()
    {
        ForEachInSpiral((x, y, it, val) =>
        {
            int index = board.GetIndex(x, y);
            indexcache[it] = index;
            nthcache[index] = it;

            var (x2, y2) = board.GetXYFromIndex(index);
            //GD.Print($"spi: {x},{y}");
        });
    }
    public int GetIndexFromNth(int nth) => indexcache[nth];
    public int GetNthFromIndex(int index) => nthcache[index];

    public (int x, int y, int val) GetXYValueFromNth(int nth)
    {
        int index = indexcache[nth];
        var (x, y) = board.GetXYFromIndex(index);
        return (x, y, board.GetFromIndex(index));
    }


    //iterate over the board while spiraling
    public void ForEachInSpiral(Action<int, int, int, int> action)
    {
        int x = 0, y = 0;
        int dx = 0;
        int dy = -1;
        double max = Size * Size;
        for (int it = 0; it < max; it++)
        {
            action(x, y, it, board[x, y]);
            if (x == y || x < 0 && x == -y || x > 0 && x == 1 - y)
            {
                int tmp = dx;
                dx = -dy;
                dy = tmp;
            }
            x += dx;
            y += dy;
        }
    }


    public void SetPatternAt(int x, int y, int val)
    {
        board[x, y] = val;
    }

    public void SetPatternAt(int index, int val)
    {
        board.SetByIndex(index, val);
    }




}
