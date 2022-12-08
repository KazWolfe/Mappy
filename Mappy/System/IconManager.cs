﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Logging;
using Dalamud.Utility;
using ImGuiScene;

namespace Mappy.System;

public class IconManager : IDisposable 
{
    private readonly Dictionary<uint, TextureWrap?> iconTextures = new();

    private const string IconFilePath = "ui/icon/{0:D3}000/{1:D6}_hr1.tex";
    
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
                var path = IconFilePath.Format(iconId / 1000, iconId);
                
                var tex = Service.DataManager.GetImGuiTexture(path);

                if (tex is not null && tex.ImGuiHandle != IntPtr.Zero) 
                {
                    iconTextures[iconId] = tex;
                } 
                else 
                {
                    tex?.Dispose();
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