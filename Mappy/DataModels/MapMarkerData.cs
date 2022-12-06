using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Dalamud.Utility;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.Utilities;
using Action = System.Action;

namespace Mappy.DataModels;

public class MapMarkerData
{
    private readonly MapMarker data;
    private Vector2 Position => new(data.X, data.Y);
    public TextureWrap? Icon => Service.Cache.IconCache.GetIconTexture(data.Icon);
    private PlaceName PlaceName => Service.Cache.PlaceNameCache.GetRow(data.PlaceNameSubtext.Row);
    private Map DataMap => Service.Cache.MapCache.GetRow(data.DataKey);
    private Aetheryte DataAetheryte => Service.Cache.AetheryteCache.GetRow(data.DataKey);
    private PlaceName DataPlaceName => Service.Cache.PlaceNameCache.GetRow(data.DataKey);
    private byte DataType => data.DataType;
    
    [MemberNotNullWhen(true, nameof(Icon))]
    private bool HasIcon => Icon != null && data.Icon != 0;
    
    public MapMarkerData(MapMarker marker)
    {
        data = marker;
    }

    public void Draw()
    {
        if (!HasIcon) return;
        
        MapRenderer.DrawIcon(Icon, Position);
        DrawTooltip();
        OnClick();
    }

    private void DrawTooltip()
    {
        if (!HasIcon) return;
        if (!ImGui.IsItemHovered()) return;

        var displayString = GetDisplayString();
        
        if (displayString is not null && displayString != string.Empty)
        {
            ImGui.BeginTooltip();
            ImGui.TextColored(GetDisplayColor(), displayString);
            ImGui.EndTooltip();
        }
    }

    private void OnClick()
    {
        if (!ImGui.IsItemClicked()) return;

        GetClickAction()?.Invoke();
    }

    private Action? GetClickAction()
    {
        return DataType switch
        {
            0 => null, // Standard Map Marker
            1 => () => Service.MapManager.LoadMap(DataMap.RowId), // Map Link
            2 => null, // Instance Links
            3 => () => Service.Teleporter.Teleport(DataAetheryte), // Aetherytes
            4 => null, // Aethernet
            _ => null
        };
    }
    
    private string? GetDisplayString()
    {
        return DataType switch
        {
            0 => GetStandardMarkerString(), // Standard Map Marker
            1 => PlaceName.Name.ToDalamudString().TextValue, // Map Link
            2 => DataMap.PlaceName.Value?.Name.ToDalamudString().TextValue, // Instance Links
            3 => DataAetheryte.PlaceName.Value?.Name.ToDalamudString().TextValue, // Aetherytes
            4 => DataPlaceName.Name.ToDalamudString().TextValue, // Aethernet
            _ => null
        };
    }

    private Vector4 GetDisplayColor()
    {
        return DataType switch
        {
            0 => Colors.White, // Standard Map Marker
            1 => Colors.MapTextBrown, // Map Link
            2 => Colors.Orange, // Instance Links
            3 => Colors.Blue, // Aetherytes
            4 => Colors.BabyBlue, // Aethernet
            _ => Colors.White
        };
    }

    private string? GetStandardMarkerString()
    {
        var placeName = PlaceName.Name.ToDalamudString().TextValue;
        if (placeName != string.Empty) return placeName;

        var mapSymbol = Service.Cache.MapSymbolCache.GetRow(data.Icon);
        return mapSymbol.PlaceName.Value?.Name.ToDalamudString().TextValue;
    }
}