using Godot;
using System;


namespace Abbaye.script.content;
public static class UpgradePatterns {

    public static int[] GetUpgradePsy_(AttackPattern pat, int lvl, int index) {
        int nth = pat.GetNthFromIndex(index);
        lvl += 1;
        if (nth == 0) {
            nth = 1;
        }
        int[] rtn = new int[2 + lvl / 2];
        for (int iter = 0; iter < rtn.Length; iter++) {
            int patindex = nth + iter;
            if (patindex < AttackPattern.Length) {
                rtn[iter] = pat.GetIndexFromNth(patindex);
            }
        }
        return rtn;
    }

    public static int[] GetUpgradeDark(AttackPattern pat, int lvl, int index) {
        int nth = pat.GetNthFromIndex(index);
        lvl++;
        if (nth == 0) {
            nth = 1;
        }

        int[] rtn = new int[1 + lvl / 4];
        rtn[0] = pat.GetIndexFromNth(nth);
        for (int iter = 1; iter < rtn.Length; iter++) {
            rtn[iter] = pat.GetIndexFromNth(GD.RandRange(1, AttackPattern.Length - 1));

        }
        return rtn;
    }

    public static int[] GetUpgradeFire(AttackPattern pat, int lvl, int index) {
        int nth = pat.GetNthFromIndex(index);
        if (nth == 0) {
            nth = 1;
        }
        int[] rtn = new int[1 + lvl / 5];
        rtn[0] = pat.GetIndexFromNth(nth);
        for (int iter = 1; iter < rtn.Length; iter++) {
            nth += 5;
            nth = Math.Min(nth, AttackPattern.Length - 1);
            rtn[iter] = pat.GetIndexFromNth(nth);
        }
        return rtn;
    }


}
public enum AttackType { Dark, Psy_, Fire, None }
