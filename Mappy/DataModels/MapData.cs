using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Dalamud.Logging;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;

namespace Mappy.DataModels;

public class MapData : IDisposable
{
    public TextureWrap? Texture { get; private set; }
    public Map? Map { get; private set; }
    public MapViewport Viewport { get; } = new();
    
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
        
        PluginLog.Debug($"Loading Map ID: {GetMapIdString(path)}");
    }
    
    private string GetMapIdString(string path) => path[7..14];
    private TextureWrap? GetMapTexture(string path) => Service.DataManager.GetImGuiTexture(path);
    private Map? GetMap(string mapIdString) => GetMapSheet().Where(map => map.Id.RawString == mapIdString).FirstOrDefault();
    private IEnumerable<Map> GetMapSheet() => Service.DataManager.GetExcelSheet<Map>()!;
    public Vector2 GetScaledMapTextureSize() => GetMapTextureSize() * Viewport.Scale;
    public Vector2 GetHalfMapTextureSize() => GetMapTextureSize() / 2.0f;
    public Vector2 GetScaledHalfMapTextureSize() => GetHalfMapTextureSize() * Viewport.Scale;
    
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
}