using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Dalamud.Utility;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.Utilities;
using Action = System.Action;

namespace Mappy.DataModels;

public enum MapMarkerType
{
    Standard,
    MapLink,
    InstanceLink,
    Aetheryte,
    Aethernet
}

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
        return (MapMarkerType?) DataType switch
        {
            MapMarkerType.Standard => null,
            MapMarkerType.MapLink => MapLinkAction,
            MapMarkerType.InstanceLink => null,
            MapMarkerType.Aetheryte => AetheryteAction,
            MapMarkerType.Aethernet => null,
            _ => null
        };
    }
    
    private string? GetDisplayString()
    {
        return (MapMarkerType?) DataType switch
        {
            MapMarkerType.Standard => GetStandardMarkerString(),
            MapMarkerType.MapLink => PlaceName.Name.ToDalamudString().TextValue,
            MapMarkerType.InstanceLink => DataMap.PlaceName.Value?.Name.ToDalamudString().TextValue,
            MapMarkerType.Aetheryte => DataAetheryte.PlaceName.Value?.Name.ToDalamudString().TextValue,
            MapMarkerType.Aethernet => DataPlaceName.Name.ToDalamudString().TextValue,
            _ => null
        };
    }

    private Vector4 GetDisplayColor()
    {
        return (MapMarkerType?) DataType switch
        {
            MapMarkerType.Standard => Colors.White,
            MapMarkerType.MapLink => Colors.MapTextBrown,
            MapMarkerType.InstanceLink => Colors.Orange,
            MapMarkerType.Aetheryte => Colors.Blue,
            MapMarkerType.Aethernet => Colors.White,
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

    private void MapLinkAction()
    {
        Service.MapManager.LoadMap(DataMap.RowId);
        Service.Configuration.FollowPlayer.Value = false;
    }

    private void AetheryteAction()
    {
        Service.Teleporter.Teleport(DataAetheryte);
    }
}