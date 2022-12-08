﻿using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Mappy.Interfaces;
using Mappy.UserInterface.Components;
using Mappy.UserInterface.Windows.ConfigurationComponents;

namespace Mappy.UserInterface.Windows;

public class ConfigurationWindow : Window
{
    private readonly SelectionFrame selectionFrame;
    private readonly ConfigurationFrame configurationFrame;

    private readonly List<ISelectable> selectables = new()
    {
        new GeneralOptions(),
        new PlayerOptions(),
        
    };

    public ConfigurationWindow() : base("Mappy Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(550, 300),
            MaximumSize = new Vector2(9999,9999)
        };

        selectionFrame = new SelectionFrame(selectables, 0.30f);
        configurationFrame = new ConfigurationFrame();
        
        IsOpen = true;
    }

    public override void Draw()
    {
        selectionFrame.Draw();

        configurationFrame.Draw(selectionFrame.Selected);
    }
}