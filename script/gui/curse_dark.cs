using Godot;
using System;

namespace Abbaye.script;

public partial class CurseDark : Control {

    bool mouse_over = false;

    ColorRect? mainrect;


    public override void _Ready() {
        mainrect = GetNode<ColorRect>("MainRect");


        mainrect.MouseEntered += OnMouseEntered;
        mainrect.MouseExited += OnMouseExited;
    }


    public delegate int OnChosenEventHandler();

    public OnChosenEventHandler? OnChosen { get; set; }

    public void OnMouseEntered() {
        mouse_over = true;
        mainrect!.Color = new Color(0.827f, 0.753f, 0.824f);
    }


    public void OnMouseExited() {
        mouse_over = false;
        mainrect!.Color = new Color(0.827f, 0.753f, 0.824f);
    }
}
