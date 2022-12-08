using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Utilities;

namespace Mappy.UserInterface.Components;

public record IconSelection(uint IconID, bool Enabled)
{
    public bool Enabled { get; set; } = Enabled;
}

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
                    
                    totalSize += iconSize.X;
                    
                    if (ImGui.IsItemClicked())
                    {
                        result.Value = iconId;
                        Service.Configuration.Save();
                    }
                    
                    if (totalSize + iconSize.X < owner.InnerWidth)
                    {
                        ImGui.SameLine();
                    }
                    else if(totalSize + iconSize.X >= owner.InnerWidth)
                    {
                        totalSize = 0.0f;
                        ImGuiHelpers.ScaledDummy(2.0f);
                    }
                }
            }
        });

        return this;
    }

    public InfoBoxIconFlexGrid MultiSelect(IEnumerable<Setting<IconSelection>> selections)
    {
        owner.AddAction(() =>
        {
            var totalSize = 0.0f;
            
            foreach (var selection in selections)
            {
                if (Service.Cache.IconCache.GetIconTexture(selection.Value.IconID) is { } icon)
                {
                    var iconSize = new Vector2(icon.Width, icon.Height) * 0.75f;
                                    
                    var cursorPosition = ImGui.GetCursorScreenPos();
                    ImGui.Image(icon.ImGuiHandle, iconSize);
                    ImGui.GetWindowDrawList().AddRect(cursorPosition, cursorPosition + iconSize,
                        selection.Value.Enabled ? ImGui.GetColorU32(Colors.SoftGreen) : ImGui.GetColorU32(Colors.Red), 5.0f,
                        ImDrawFlags.RoundCornersAll, 5.0f);

                    totalSize += iconSize.X;
                    
                    if (ImGui.IsItemClicked())
                    {
                        selection.Value.Enabled = !selection.Value.Enabled;
                        Service.Configuration.Save();
                    }
                    
                    if (totalSize + iconSize.X < owner.InnerWidth)
                    {
                        ImGui.SameLine();
                    }
                    else if(totalSize + iconSize.X >= owner.InnerWidth)
                    {
                        totalSize = 0.0f;
                        ImGuiHelpers.ScaledDummy(2.0f);
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