using Godot;
using System;


namespace Abbaye.script.content;
public static class UpgradePatterns
{

    public static int[] GetUpgradePsy(AttackPattern pat, int lvl, int index)
    {
        int nth = pat.GetNthFromIndex(index);
        lvl += 1;
        if (nth == 0)
        {
            nth = 1;
        }
        int[] rtn = new int[lvl * 3];
        for (int iter = 0; iter < rtn.Length; iter++)
        {
            int patindex = nth + iter;
            if (patindex < AttackPattern.Length)
            {
                rtn[iter] = pat.GetIndexFromNth(patindex);
            }
        }
        GD.Print($"lvl:{lvl},rtn:{rtn},len:{rtn.Length}");
        return rtn;
    }

    public static int[] GetUpgradeDark(AttackPattern pat, int lvl, int index)
    {
        int nth = pat.GetNthFromIndex(index);
        lvl++;
        if (nth == 0)
        {
            nth = GD.RandRange(1, AttackPattern.Length);
        }

        int[] rtn = new int[1 + (int)(lvl / 2f)];
        rtn[0] = pat.GetIndexFromNth(nth);
        for (int iter = 1; iter < rtn.Length; iter++)
        {
            rtn[iter] = GD.RandRange(1, AttackPattern.Length);

        }
        return rtn;
    }

    public static int[] GetUpgradeFire(AttackPattern pat, int lvl, int index)
    {
        int nth = pat.GetNthFromIndex(index);
        if (nth == 0)
        {
            nth = 1;
        }
        int[] rtn = new int[1 + (int)(1 + lvl / 3f)];
        rtn[0] = pat.GetIndexFromNth(nth);
        for (int iter = 1; iter < rtn.Length; iter++)
        {
            nth += 10;
            nth = Math.Min(nth, AttackPattern.Length - 1);
            rtn[iter] = pat.GetIndexFromNth(nth);
        }
        return rtn;
    }


}
