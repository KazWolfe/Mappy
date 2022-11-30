using System;
using System.Text;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.MapComponents;

namespace Mappy.System;

public unsafe class MapManager : IDisposable
{
    public MapData MapData { get; } = new();
    public bool FollowPlayer { get; set; }
    
    private readonly PlayerMapComponent player = new();
    private readonly GatheringPointMapComponent gatheringPoints = new();
    private readonly MapMarkersMapComponent mapMarkers = new();

    [Signature("48 8D 15 ?? ?? ?? ?? 48 83 C1 08 44 8B C7", ScanType = ScanType.StaticAddress)]
    private readonly byte* mapPath = null!;
    
    private string lastMapPath = string.Empty;

    public MapManager()
    {
        SignatureHelper.Initialise(this);

        Service.Framework.Update += OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (Service.Condition[ConditionFlag.BetweenAreas] || Service.Condition[ConditionFlag.BetweenAreas51]) return;

        if (mapPath is not null)
        {
            var pathString = Encoding.UTF8.GetString(mapPath + 0x1011CB, 27).Trim('\0');

            if (pathString == string.Empty) return;
            
            if (lastMapPath != pathString)
            {
                PluginLog.Debug($"Map Path Updated: {pathString}");
                UpdateCurrentMap(pathString);
                lastMapPath = pathString;
            }
        }
    }

    public void Dispose()
    {
        MapData.Dispose();
        
        Service.Framework.Update -= OnFrameworkUpdate;
    }
    
    private void UpdateCurrentMap(string mapTexturePath)
    {
        MapData.LoadMap(mapTexturePath);

        mapMarkers.LoadMarkers();
    }
    
    public void DrawMap()
    {
        if (!MapData.DataAvailable) return;
        
        if (FollowPlayer)
        {
            CenterOnPlayer();
        }
            
        DrawMapImage();

        mapMarkers.Draw();
        gatheringPoints.Draw();
        player.Draw();
    }

    private void CenterOnPlayer()
    {
        if (!MapData.DataAvailable) return;
        
        if (Service.ClientState.LocalPlayer is { } localPlayer)
        {
            var playerPosition = MapData.GetGameObjectPosition(localPlayer.Position);
            
            MapData.Viewport.Center = playerPosition;
        }
    }
    
    private void DrawMapImage()
    {
        if (!MapData.DataAvailable) return;
        
        var textureSize = MapData.GetScaledMapTextureSize();
        
        MapData.SetDrawPosition();
        ImGui.Image(MapData.Texture.ImGuiHandle, textureSize);
    }
}