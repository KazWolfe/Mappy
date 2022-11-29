using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Text;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.DataStructures;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System;

public unsafe class MapManager : IDisposable
{
    public TextureWrap? CurrentMapTexture { get; private set; } = null;
    public MapViewport MapViewport { get; set; } = new();
    public Map? CurrentMapInfo { get; set; } = null;
    public TerritoryType? TerritoryInfo { get; set; } = null;

    private List<MapMarker> mapMarkers = new();

    private bool followPlayer = false;
    
    private Vector2 LastWindowSize = Vector2.Zero;

    private readonly GatheringPointName mineralDeposit;
    private readonly GatheringPointName rockyOutcrop;
    private readonly GatheringPointName matureTree;
    private readonly GatheringPointName lushVegetation;

    [Signature("8B 2D ?? ?? ?? ?? 41 BF", ScanType = ScanType.StaticAddress)]
    private readonly TerritoryInfoStruct* territoryStruct = null!;

    [Signature("48 8D 15 ?? ?? ?? ?? 48 83 C1 08 44 8B C7", ScanType = ScanType.StaticAddress)]
    private readonly byte* mapPath = null!;
    
    private string lastMapPath = string.Empty;

    public MapManager()
    {
        SignatureHelper.Initialise(this);
        
        mineralDeposit = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(1)!;
        rockyOutcrop = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(2)!;
        matureTree = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(3)!;
        lushVegetation = Service.DataManager.GetExcelSheet<GatheringPointName>()!.GetRow(4)!;
        
        Service.ClientState.TerritoryChanged += OnZoneChange;
        
        UpdateCurrentMap();

        Service.Framework.Update += OnFrameworkUpdate;
    }

    private void OnFrameworkUpdate(Framework framework)
    {
        if (mapPath is not null)
        {
            var pathString = Encoding.UTF8.GetString(mapPath + 0x1011CB, 27);

            if (lastMapPath != pathString)
            {
                var mapIndex = pathString[7..14];
                
                PluginLog.Debug($"MapIndex: {mapIndex}");
                
                UpdateCurrentMap(pathString, mapIndex);

                lastMapPath = pathString;
                
                PluginLog.Debug($"Map Path Updated: {pathString}");
            }
        }
    }

    public void Dispose()
    {
        CurrentMapTexture?.Dispose();
        
        Service.ClientState.TerritoryChanged -= OnZoneChange;
        Service.Framework.Update -= OnFrameworkUpdate;
    }

    private void OnZoneChange(object? sender, ushort e)
    {
        UpdateCurrentMap();
    }

    private static bool GetCurrentTerritoryType([NotNullWhen(true)] out TerritoryType? territoryType)
    {
        var currentTerritoryID = Service.ClientState.TerritoryType;

        territoryType = Service.DataManager.GetExcelSheet<TerritoryType>()!.GetRow(currentTerritoryID);

        return territoryType != null;
    }

    private static bool GetMapTexture(TerritoryType territory, [NotNullWhen(true)] out TextureWrap? textureWrap)
    {
        textureWrap = null;

        if (GetTerritoryMap(territory, out var map))
        {
            var path = $"ui/map/{map.Id}/{territory.Name}00_m.tex";
            PluginLog.Debug(path);

            textureWrap = Service.DataManager.GetImGuiTexture(path);

            return textureWrap != null;
        }
        
        return false;
    }

    private static bool GetMapTexture(string mapPath, [NotNullWhen(true)] out TextureWrap? textureWrap)
    {
        textureWrap = Service.DataManager.GetImGuiTexture(mapPath);

        return textureWrap != null;
    }

    private static bool GetTerritoryMap(TerritoryType territory, [NotNullWhen(true)] out Map? map)
    {
        map = territory.Map.Value;

        return map != null;
    }
    
    private static bool GetTerritoryMap(string mapID, [NotNullWhen(true)] out Map? map)
    {
        map = Service.DataManager.GetExcelSheet<Map>()!
            .Where(map => map.Id.RawString == mapID)
            .FirstOrDefault();
        
        return map != null;
    }

    private void UpdateCurrentMap()
    {
        if (GetCurrentTerritoryType(out var territoryType))
        {
            TerritoryInfo = territoryType;
            CurrentMapTexture = GetMapTexture(territoryType, out var texture) ? texture : null;
            CurrentMapInfo = GetTerritoryMap(territoryType, out var mapInfo) ? mapInfo : null;
            
            mapMarkers.Clear();

            if (mapInfo is not null)
            {
                foreach (var row in Service.DataManager.GetExcelSheet<MapMarker>()!)
                {
                    if (row.RowId == mapInfo.MapMarkerRange)
                    {
                        mapMarkers.Add(row);
                    }
                }
            }
        }
        else
        {
            TerritoryInfo = null;
        }
    }

    private void UpdateCurrentMap(string mapPath, string mapId)
    {
        CurrentMapTexture = GetMapTexture(mapPath, out var texture) ? texture : null;
        CurrentMapInfo = GetTerritoryMap(mapId, out var mapInfo) ? mapInfo : null;
            
        mapMarkers.Clear();

        if (mapInfo is not null)
        {
            PluginLog.Debug($"MarkerRange: {mapInfo.MapMarkerRange}");
                
            foreach (var row in Service.DataManager.GetExcelSheet<MapMarker>()!)
            {
                if (row.RowId == mapInfo.MapMarkerRange)
                {
                    mapMarkers.Add(row);
                }
            }
        }
    }
    
    public void DrawMap()
    {
        if (CurrentMapTexture is not null)
        {
            if (followPlayer)
            {
                CenterOnPlayer();
            }
            
            DrawMapImage(CurrentMapTexture, MapViewport);

            DrawMapMarkers();

            DrawGatheringNodes();
            
            DrawPlayer();
        }
    }

    private void DrawGatheringNodes()
    {
        foreach (var obj in Service.ObjectTable)
        {
            if(obj.ObjectKind != ObjectKind.GatheringPoint) continue;
            
            var iconId = GetIconIdForGatheringNode(obj);
            
            if (Service.IconManager.GetIconTexture(iconId) is {} icon)
            {
                DrawGatheringMarker(icon, obj.Position);
            }
        }
    }

    private uint GetIconIdForGatheringNode(GameObject gatheringNode)
    {
        var iconId = 0u;
        var nodeName = gatheringNode.Name.TextValue.ToLower();
        
        if (nodeName == mineralDeposit.Singular)
        {
            iconId = 60438;
        }
        else if (nodeName == rockyOutcrop.Singular)
        {
            iconId = 60437;
        }
        else if (nodeName == matureTree.Singular)
        {
            iconId = 60433;
        }
        else if (nodeName == lushVegetation.Singular)
        {
            iconId = 60432;
        }

        return iconId;
    }

    private void CenterOnPlayer()
    {
        if (Service.ClientState.LocalPlayer is { } player)
        {
            if (CurrentMapInfo is not null)
            {
                if (CurrentMapTexture is not null)
                {
                    var textureSize = GetTextureSize(CurrentMapTexture) / 2.0f;
                    
                    var playerPosition = new Vector2(player.Position.X, player.Position.Z) * CurrentMapInfo.SizeFactor / 100.0f;

                    MapViewport.Center = playerPosition + textureSize;
                }
            }
        }
    }

    private void DrawMapMarkers()
    {
        foreach (var marker in mapMarkers)
        {
            DrawMapMarker(marker, MapViewport);
        }
    }

    private void DrawMapMarker(MapMarker marker, MapViewport viewport)
    {
        var icon = Service.IconManager.GetIconTexture(marker.Icon);

        if (icon is not null)
        {
            var iconSize = new Vector2(icon.Width, icon.Height);
            var viewportPosition = viewport.Center * viewport.Scale - viewport.Size / 2.0f;

            var markerCenter = new Vector2(marker.X, marker.Y) * viewport.Scale - iconSize / 2.0f;

            // if (marker.PlaceNameSubtext.Value is { } placeName && viewport.Scale > 0.5f)
            // {
            //     var stringSize = new Vector2(ImGui.CalcTextSize(placeName.Name).X, 0.0f) * viewport.Scale;
            //     var textOffset = new Vector2(0.0f, -20.0f);
            //     
            //     ImGui.SetCursorPos(-viewportPosition + markerCenter - stringSize);
            //     ImGui.TextColored(Colors.Black, placeName.Name.ToDalamudString().TextValue);
            // }
            
            ImGui.SetCursorPos(-viewportPosition + markerCenter);
            ImGui.Image(icon.ImGuiHandle, iconSize);
        }
    }

    private void DrawGatheringMarker(TextureWrap icon, Vector3 position)
    {
        if (CurrentMapInfo is not null && CurrentMapTexture is not null)
        {
            var iconSize = new Vector2(icon.Width, icon.Height);
            var texturePosition = new Vector2(position.X, position.Z) * CurrentMapInfo.SizeFactor / 100.0f * MapViewport.Scale;
            var textureCentered = texturePosition - iconSize / 2.0f;
            
            var mapSize = GetTextureSize(CurrentMapTexture) * MapViewport.Scale;
            var viewportPosition = MapViewport.Center * MapViewport.Scale - MapViewport.Size / 2.0f;

            ImGui.SetCursorPos(-viewportPosition + textureCentered + mapSize / 2.0f);
            ImGui.Image(icon.ImGuiHandle, iconSize);
        }
    }

    private static Vector2 ImRotate(Vector2 v, float cosA, float sinA) 
    { 
        return new Vector2(v.X * cosA - v.Y * sinA, v.X * sinA + v.Y * cosA);
    }
    
    private void DrawPlayer()
    {
        if (Service.ClientState.LocalPlayer is { } player)
        {
            if (CurrentMapInfo is not null)
            {
                if (CurrentMapTexture != null)
                {
                    var viewport = MapViewport;
                    var viewportPosition = viewport.Center * viewport.Scale - viewport.Size / 2.0f;

                    var textureSize = GetTextureSize(CurrentMapTexture) / 2.0f * viewport.Scale;
                    
                    var playerPosition = new Vector2(player.Position.X, player.Position.Z) * CurrentMapInfo.SizeFactor / 100.0f * viewport.Scale;
                    
                    // ImGui.GetWindowDrawList().AddCircleFilled(ImGui.GetWindowPos() - viewportPosition + playerPosition + textureSize, 5.0f, ImGui.GetColorU32(Colors.Red));

                    var cameraManager = CameraManager.Instance()->CurrentCamera;
                        
                    var yaw = -1 * Math.Atan2(-1 * cameraManager->Vector_4.X, -1 * cameraManager->Vector_2.X);

                    // Reverse the angle and offset by 90 degrees
                    var angle = -((float) yaw - 0.5f * MathF.PI);

                    var start = ImGui.GetWindowPos() - viewportPosition + playerPosition + textureSize;
                    const float lineLength = 30.0f;
                    var stop = new Vector2(lineLength * MathF.Cos(angle), lineLength * MathF.Sin(angle));

                    // ImGui.GetWindowDrawList().AddLine(start, start + stop, ImGui.GetColorU32(Colors.Red), 5.0f);

                    if (Service.IconManager.GetIconTexture(60443) is { } playerIcon)
                    {
                        var size = new Vector2(playerIcon.Width, playerIcon.Height) * 1.25f;
                        
                        var cosA = MathF.Cos(angle + 0.5f * MathF.PI);
                        var sinA = MathF.Sin(angle + 0.5f * MathF.PI);

                        var center = ImGui.GetWindowPos() - viewportPosition + playerPosition + textureSize;
                        
                        Vector2[] vectors =
                        {
                            center + ImRotate(new Vector2(-size.X * 0.5f, -size.Y * 0.5f), cosA, sinA),
                            center + ImRotate(new Vector2(+size.X * 0.5f, -size.Y * 0.5f), cosA, sinA),
                            center + ImRotate(new Vector2(+size.X * 0.5f, +size.Y * 0.5f), cosA, sinA),
                            center + ImRotate(new Vector2(-size.X * 0.5f, +size.Y * 0.5f), cosA, sinA)
                        };
                        
                        ImGui.GetWindowDrawList().AddImageQuad(playerIcon.ImGuiHandle, vectors[0], vectors[1], vectors[2], vectors[3]);
                        
                        // ImGui.SetCursorPos(-viewportPosition + playerPosition + textureSize - iconSize / 2.0f);
                        // ImGui.Image(playerIcon.ImGuiHandle, iconSize);
                    }
                }


            }
        }
    }
    
    private void DrawMapImage(TextureWrap texture, MapViewport viewport)
    {
        var textureSize = GetTextureSize(texture) * viewport.Scale;
        var viewportPosition = viewport.Center * viewport.Scale - viewport.Size / 2.0f;

        ImGui.SetCursorPos(-viewportPosition);
        ImGui.Image(texture.ImGuiHandle, textureSize, Vector2.Zero, Vector2.One);
        
        DebugWindow.AddString("DrawMapImage:");
        DebugWindow.AddString($"Texture Size: {textureSize}");
        DebugWindow.AddString($"Viewport Position: {viewportPosition}");
        DebugWindow.AddString($"Viewport Center: {viewport.Center}");
        DebugWindow.AddString($"Scale: {viewport.Scale}");
    }

    public void ZoomIn(float scaleValue)
    {
        if (CurrentMapTexture is not null)
        {
            MapViewport.Scale += scaleValue;
        }
    }

    public void ZoomOut(float scaleValue)
    {
        if (CurrentMapTexture is not null)
        {
            var newScale = MapViewport.Scale - scaleValue;

            if (newScale > 0.2f)
            {
                MapViewport.Scale -= scaleValue;
            }
        }
    }

    private Vector2 GetTextureSize(TextureWrap texture) => new(texture.Width, texture.Height);
    private Vector2 GetPlayerPosition(Vector3 position) => new(position.X, position.Y);
}