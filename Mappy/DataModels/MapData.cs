using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;

namespace Mappy.DataModels;

public class MapData : IDisposable
{
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
        var newPath = $"ui/map/{mapKey}/{rawKey}_m.tex";

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
    
    private string GetMapIdString(string path) => path[7..14];
    private TextureWrap? GetMapTexture(string path) => Service.DataManager.GetImGuiTexture(path);
    private Map? GetMap(string mapIdString) => GetMapSheet().Where(map => map.Id.RawString == mapIdString).FirstOrDefault();
    private IEnumerable<Map> GetMapSheet() => Service.DataManager.GetExcelSheet<Map>()!;
    public Vector2 GetScaledMapTextureSize() => GetMapTextureSize() * Viewport.Scale;
    public Vector2 GetHalfMapTextureSize() => GetMapTextureSize() / 2.0f;

    public string GetCurrentMapName()
    {
        if(!MapAvailable) throw new NullReferenceException("Map is null");

        if (GetPlaceName() is { } placeName) return placeName;
        if (GetPlaceSubName() is { } subName) return subName;

        return string.Empty;
    }

    private string? GetPlaceName() => !MapAvailable ? null : Map.PlaceName.Value?.Name.RawString;
    private string? GetPlaceSubName() => !MapAvailable ? null : Map.PlaceNameSub.Value?.Name.RawString;

    public bool PlayerInCurrentMap()
    {
        if (!MapAvailable) return false;

        return Map.Id.RawString == playerMapKey;
    }

    public Vector2 GetMapTextureSize()
    {
        if(!TextureAvailable) throw new NullReferenceException("Texture is null");
        
        return new Vector2(Texture.Width, Texture.Height);
    }

    public float GetMapScalar()
    {
        if (!MapAvailable) throw new NullReferenceException("Map is null");

        return Map.SizeFactor / 100.0f;
    }
    
    public Vector2 GetScaledMapOffset()
    {
        if (!MapAvailable) throw new NullReferenceException("Map is null");

        return new Vector2(Map.OffsetX, Map.OffsetY) * GetMapScalar();
    }

    public Vector2 GetScaledGameObjectPosition(Vector3 objectPosition) => GetGameObjectPosition(objectPosition) * Viewport.Scale;

    public Vector2 GetGameObjectPosition(Vector3 objectPosition) =>
        new Vector2(objectPosition.X, objectPosition.Z) * GetMapScalar()
        + GetScaledMapOffset()
        + GetHalfMapTextureSize();
    
    public Vector2 GetScaledPosition(Vector2 texturePosition) => new Vector2(texturePosition.X, texturePosition.Y) * Viewport.Scale;

    public void SetDrawPosition(Vector2 texturePosition) => ImGui.SetCursorPos(-Viewport.ScaledTopLeft + texturePosition);
    public void SetDrawPosition() => ImGui.SetCursorPos(-Viewport.ScaledTopLeft);
    public Vector2 GetWindowDrawPosition(Vector2 texturePosition) => -Viewport.ScaledTopLeft + texturePosition + ImGui.GetWindowPos();
}