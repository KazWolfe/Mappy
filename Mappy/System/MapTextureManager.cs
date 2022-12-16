using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Logging;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;

namespace Mappy.System;

public class MapTextureManager : IDisposable
{
    private readonly Dictionary<uint, TextureWrap?> mapTextures = new();
    
    public void Dispose() 
    {
        foreach (var texture in mapTextures.Values) 
        {
            texture?.Dispose();
        }

        mapTextures.Clear();
    }
        
    private void LoadMapTexture(uint mapId) 
    {
        Task.Run(() => 
        {
            try 
            {
                var map = Service.Cache.MapCache.GetRow(mapId);
                var path = GetPathFromMap(map);
                var tex = Service.Penumbra.GetTexture(path);
                
                if (tex is not null && tex.ImGuiHandle != IntPtr.Zero) 
                {
                    mapTextures[mapId] = tex;
                } 
                else 
                {
                    tex?.Dispose();
                }
            } 
            catch (Exception ex) 
            {
                PluginLog.LogError($"Failed loading texture for icon {mapId} - {ex.Message}");
            }
        });
    }
    
    public TextureWrap? GetMapTexture(uint mapId) 
    {
        if (mapTextures.ContainsKey(mapId)) return mapTextures[mapId];

        mapTextures.Add(mapId, null);
        LoadMapTexture(mapId);

        return mapTextures[mapId];
    }
    
    private static string GetPathFromMap(Map map)
    {
        var mapKey = map.Id.RawString;
        var rawKey = mapKey.Replace("/", "");
        return $"ui/map/{mapKey}/{rawKey}_m.tex";
    }
}