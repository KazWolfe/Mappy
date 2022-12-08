using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Utilities;

namespace Mappy.UserInterface.Components;


public class InfoBoxIconFlexGrid
{
    private readonly InfoBox owner;
    
    public InfoBoxIconFlexGrid(InfoBox owner)
    {
        this.owner = owner;
    }

    public InfoBoxIconFlexGrid SingleSelect(Setting<uint> result, params uint[] iconOptions)
    {
        owner.AddAction(() =>
        {
            var totalSize = 0.0f;
            
            foreach (var iconId in iconOptions)
            {
                if (Service.Cache.IconCache.GetIconTexture(iconId) is { } icon)
                {
                    var iconSize = new Vector2(icon.Width, icon.Height);
                                    
                    if (totalSize + iconSize.X < owner.InnerWidth && totalSize != 0)
                    {
                        ImGui.SameLine();
                        totalSize += iconSize.X;
                    }
                    else if (totalSize == 0)
                    {
                        totalSize += iconSize.X;
                    }
                    else
                    {
                        totalSize = 0.0f;
                    }

                    if (result.Value == iconId)
                    {
                        var cursorPosition = ImGui.GetCursorScreenPos();
                        ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, Vector4.One);
                        ImGui.GetWindowDrawList().AddRect(cursorPosition,  cursorPosition + iconSize, ImGui.GetColorU32(Colors.Green), 5.0f, ImDrawFlags.RoundCornersAll, 5.0f);
                    }
                    else
                    {
                        ImGui.Image(icon.ImGuiHandle, iconSize, Vector2.Zero, Vector2.One, Vector4.One * 0.5f);
                    }
                    
                    if (ImGui.IsItemClicked())
                    {
                        result.Value = iconId;
                    }
                }
            }
        });

        return this;
    }

    public InfoBox EndFlexGrid()
    {
        return owner;
    }
}