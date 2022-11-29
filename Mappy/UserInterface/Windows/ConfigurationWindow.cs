using System.Numerics;
using Dalamud.Interface.Windowing;

namespace Mappy.UserInterface.Windows;

public class ConfigurationWindow : Window
{
    public ConfigurationWindow() : base("Mappy Configuration")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(350, 350),
            MaximumSize = new Vector2(9999,9999)
        };
    }

    public override void Draw()
    {
        
    }
}