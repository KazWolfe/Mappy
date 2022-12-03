using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.Utilities;

namespace Mappy.DataModels;

public class MapData : IDisposable
{
    // Properties
    public TextureWrap? Texture { get; private set; }
    public Map? Map { get; private set; }
    public MapViewport Viewport { get; } = new();
    private string playerMapKey = string.Empty;

    public List<Map> MapLayers { get; private set; } = new();
    
    [MemberNotNullWhen(true, nameof(Texture))]
    [MemberNotNullWhen(true, nameof(Map))]
    public bool DataAvailable => TextureAvailable && MapAvailable;
    
    [MemberNotNullWhen(true, nameof(Texture))]
    private bool TextureAvailable => Texture != null;
    
    [MemberNotNullWhen(true, nameof(Map))]
    private bool MapAvailable => Map != null;
    
    public void Dispose()
    {
        Texture?.Dispose();
    }

    // Helpers
    private static string GetMapIdString(string path) => path[7..14];
    private static TextureWrap? GetMapTexture(string path) => Service.DataManager.GetImGuiTexture(path + ".tex");
    private static IEnumerable<Map> GetMapSheet() => Service.DataManager.GetExcelSheet<Map>()!;
    private static Map? GetMap(string mapIdString) => GetMapSheet().Where(map => map.Id.RawString == mapIdString).FirstOrDefault();
    public Vector2 GetScaledMapTextureSize() => GetMapTextureSize() * Viewport.Scale;
    public Vector2 GetHalfMapTextureSize() => GetMapTextureSize() / 2.0f;
    private string? GetPlaceName() => !MapAvailable ? null : Map.PlaceName.Value?.Name.RawString;
    private string? GetPlaceSubName() => !MapAvailable ? null : Map.PlaceNameSub.Value?.Name.RawString;
    public Vector2 GetScaledGameObjectPosition(Vector3 objectPosition) => GetGameObjectPosition(objectPosition) * Viewport.Scale;
    public void SetDrawPosition(Vector2 texturePosition) => ImGui.SetCursorPos(-Viewport.ScaledTopLeft + texturePosition);
    public void SetDrawPosition() => ImGui.SetCursorPos(-Viewport.ScaledTopLeft);
    public Vector2 GetWindowDrawPosition(Vector2 texturePosition) => -Viewport.ScaledTopLeft + texturePosition + ImGui.GetWindowPos();
    public Vector2 GetScaledWindowDrawPosition(Vector2 texturePosition) => -Viewport.ScaledTopLeft + texturePosition * Viewport.Scale + ImGui.GetWindowPos();
    public Vector2 GetGameObjectPosition(Vector3 objectPosition) =>
        new Vector2(objectPosition.X, objectPosition.Z) * GetMapScalar()
        + GetScaledMapOffset()
        + GetHalfMapTextureSize();
    
    // Methods
    
    public void LoadMap(string path)
    {
        Texture = GetMapTexture(path);
        Map = GetMap(GetMapIdString(path));
        playerMapKey = GetMapIdString(path);
        
        LoadMapLayers();
        
        PluginLog.Debug($"Loading Map ID: {GetMapIdString(path)}");
    }
    
    public void LoadMapWithKey(string mapKey)
    {
        var rawKey = mapKey.Replace("/", "");
        var newPath = $"ui/map/{mapKey}/{rawKey}_m";

        Texture = GetMapTexture(newPath);
        Map = GetMap(mapKey);
        
        LoadMapLayers();

        if (!TextureAvailable) return;
        Viewport.Center = new Vector2(Texture.Width, Texture.Height) / 2.0f;
    }

    public void SelectMapLayer(Map layer)
    {
        LoadMapWithKey(layer.Id.RawString);
    }

    private void LoadMapLayers()
    {
        if (!MapAvailable) return;

        MapLayers = Service.DataManager.GetExcelSheet<Map>()!
            .Where(map => map.TerritoryType.Row == Map.TerritoryType.Row)
            .ToList();
        
        PluginLog.Debug($"Loaded {MapLayers.Count} May Layers");
    }
    
    public string GetCurrentMapName()
    {
        if(!MapAvailable) throw new NullReferenceException("Map is null");

        if (GetPlaceName() is { } placeName) return placeName;
        if (GetPlaceSubName() is { } subName) return subName;

        return string.Empty;
    }

    public bool PlayerInCurrentMap()
    {
        if (!MapAvailable) return false;

        return Map.Id.RawString == playerMapKey;
    }

    private Vector2 GetMapTextureSize()
    {
        if(!TextureAvailable) throw new NullReferenceException("Texture is null");
        
        return new Vector2(Texture.Width, Texture.Height);
    }

    public float GetMapScalar()
    {
        if (!MapAvailable) throw new NullReferenceException("Map is null");

        return Map.SizeFactor / 100.0f;
    }

    private Vector2 GetScaledMapOffset()
    {
        if (!MapAvailable) throw new NullReferenceException("Map is null");

        return new Vector2(Map.OffsetX, Map.OffsetY) * GetMapScalar();
    }

    public void DrawIcon(uint iconID, short x, short y)
    {
        var icon = Service.IconManager.GetIconTexture(iconID);
        if (icon is null) return;
        
        var iconSize = new Vector2(icon.Width, icon.Height);
        var iconPosition = new Vector2(x, y) * Viewport.Scale - iconSize / 2.0f;
        
        SetDrawPosition(iconPosition);
        ImGui.Image(icon.ImGuiHandle, iconSize);
    }

    public void DrawText(PlaceName? name, short x, short y, bool region)
    {
        if (name != null)
        {
            var position = GetScaledWindowDrawPosition(new Vector2(x, y));

            var scale = region ? 1.5f : 1.0f;
            
            Draw.TextOutlined(position, name.Name.RawString, scale);
        }
    }
    
    public void DrawIcon(uint iconID, Vector3 location)
    {
        var icon = Service.IconManager.GetIconTexture(iconID);
        if (icon is null) return;

        var iconSize = new Vector2(icon.Width, icon.Height);
        var iconPosition = GetScaledGameObjectPosition(location) - iconSize / 2.0f;
        
        SetDrawPosition(iconPosition);
        ImGui.Image(icon.ImGuiHandle, iconSize);
    }
}