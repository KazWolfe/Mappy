using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Logging;
using Dalamud.Utility;
using ImGuiScene;

namespace Mappy.System;

public class IconManager : IDisposable 
{
    private readonly Dictionary<uint, TextureWrap?> iconTextures = new();

    public void Dispose() 
    {
        foreach (var texture in iconTextures.Values) 
        {
            texture?.Dispose();
        }

        iconTextures.Clear();
    }
        
    private void LoadIconTexture(uint iconId) 
    {
        Task.Run(() => 
        {
            try 
            {
                var iconTex = Service.DataManager.GetIcon(iconId);
                if (iconTex == null) return;
                
                var tex = Service.PluginInterface.UiBuilder.LoadImageRaw(iconTex.GetRgbaImageData(), iconTex.Header.Width, iconTex.Header.Height, 4);

                if (tex.ImGuiHandle != IntPtr.Zero) 
                {
                    iconTextures[iconId] = tex;
                } 
                else 
                {
                    tex.Dispose();
                }
            } 
            catch (Exception ex) 
            {
                PluginLog.LogError($"Failed loading texture for icon {iconId} - {ex.Message}");
            }
        });
    }
    
    public TextureWrap? GetIconTexture(uint iconId) 
    {
        if (iconTextures.ContainsKey(iconId)) return iconTextures[iconId];

        iconTextures.Add(iconId, null);
        LoadIconTexture(iconId);

        return iconTextures[iconId];
    }
}