using Abbaye.script.content;
using Godot;
using System;

namespace Abbaye.script;

public partial class LevelUpMenu : Panel {

    int level = 0;
    public AttackPattern? pat;
    HBoxContainer? cont;
    SelectTile? selector;

    public override void _Ready() {
        cont = GetNode<HBoxContainer>("HBoxContainer");
    }

    public static readonly PackedScene tile_selector = GD.Load<PackedScene>("res://scenes/gui/select_tile.tscn");

    public static readonly PackedScene more_curse_dark = GD.Load<PackedScene>("res://scenes/gui/curse_dark.tscn");
    public static readonly PackedScene more_curse_psy_ = GD.Load<PackedScene>("res://scenes/gui/curse_psy.tscn");
    public static readonly PackedScene more_curse_fire = GD.Load<PackedScene>("res://scenes/gui/curse_fire.tscn");



    public readonly PackedScene[] upgrades = new PackedScene[] { more_curse_dark, more_curse_psy_, more_curse_fire };

    CurseDark[] options = Array.Empty<CurseDark>();

    public void OnLevelUp(AttackPattern pattern, int nlevel) {
        pat = pattern;
        level = nlevel;
        options = GetNewOptions(3);
        foreach (var opt in options) {
            opt.OnChosen += OnChosen;
            cont!.AddChild(opt);
        }
        GetTree().Paused = true;
        Visible = true;
    }

    public CurseDark[] GetNewOptions(int amount) {
        CurseDark[] rtn = new CurseDark[amount];

        //algorithm to get X random new options in an array of Y length
        int remaining = amount;
        for (int iter = 0; iter < upgrades.Length; iter++) {
            float probability = ((float)remaining) / (upgrades.Length - iter);
            if (GD.Randf() < probability) {
                rtn[remaining - 1] = upgrades[iter].Instantiate<CurseDark>();
                remaining--;
                if (remaining == 0) { break; }
            }
        }
        return rtn;
    }

    AttackType? lasttype;

    public void OnChosen(CurseDark.OptionType type) {
        //TODO replace by a selection screen
        AttackType attype = type switch {
            CurseDark.OptionType.MoreDark => AttackType.Dark,
            CurseDark.OptionType.MoreFire => AttackType.Fire,
            CurseDark.OptionType.MorePsy_ => AttackType.Psy_,
            _ => 0
        };
        lasttype = attype;

        SelectTile selector = tile_selector.Instantiate<SelectTile>();
        selector.SetStuff(pat!, attype, level);
        selector.OnSocketChosen += OnPatternPosChosen;
        AddChild(selector);


    }

    public void OnPatternPosChosen(int[] toadd) {
        int val = lasttype switch {
            AttackType.Dark => AttackPattern.BULLET_DARK,
            AttackType.Fire => AttackPattern.BULLET_FIRE,
            AttackType.Psy_ => AttackPattern.BULLET_PSY,
            _ => 0
        };
        foreach (int pos in toadd) {
            var (x, y) = pat!.board.GetXYFromIndex(pos);
            pat!.SetPatternAt(x, y, val);
        }

        foreach (var opt in options) {
            opt.QueueFree();
        }
        this.Visible = false;
        GetTree().Paused = false;
    }



}
