using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Dalamud.Game.ClientState;
using Dalamud.Logging;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;
using ImGuiNET;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.UserInterface.Windows;
using Mappy.Utilities;

namespace Mappy.System;

public class MapManager : IDisposable
{
    public TextureWrap? CurrentMapTexture { get; private set; } = null;
    public MapViewport MapViewport { get; set; } = new();
    public Map? CurrentMapInfo { get; set; } = null;
    public TerritoryType? TerritoryInfo { get; set; } = null;

    private List<MapMarker> MapMarkers = new();

    private bool FollowPlayer = true;

    public MapManager()
    {
        Service.ClientState.TerritoryChanged += OnZoneChange;
        
        UpdateCurrentMap();
    }
    
    public void Dispose()
    {
        CurrentMapTexture?.Dispose();
        
        Service.ClientState.TerritoryChanged -= OnZoneChange;
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

    private static bool GetTerritoryMap(TerritoryType territory, [NotNullWhen(true)] out Map? map)
    {
        map = territory.Map.Value;

        return map != null;
    }

    private void UpdateCurrentMap()
    {
        if (GetCurrentTerritoryType(out var territoryType))
        {
            TerritoryInfo = territoryType;
            CurrentMapTexture = GetMapTexture(territoryType, out var texture) ? texture : null;
            CurrentMapInfo = GetTerritoryMap(territoryType, out var mapInfo) ? mapInfo : null;
            
            MapMarkers.Clear();

            if (mapInfo is not null)
            {
                foreach (var row in Service.DataManager.GetExcelSheet<MapMarker>()!)
                {
                    if (row.RowId == mapInfo.MapMarkerRange)
                    {
                        MapMarkers.Add(row);
                    }
                }
            }
        }
        else
        {
            TerritoryInfo = null;
        }
    }

    public void DrawMap()
    {
        if (CurrentMapTexture is not null)
        {
            if (FollowPlayer)
            {
                CenterOnPlayer();
            }
            
            DrawMapImage(CurrentMapTexture, MapViewport);

            DrawMapMarkers();
            
            DrawPlayer();
        }
    }

    private void CenterOnPlayer()
    {
        if (Service.ClientState.LocalPlayer is { } player)
        {
            if (CurrentMapInfo is not null)
            {
                var viewport = MapViewport;

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
        foreach (var marker in MapMarkers)
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
                
                    DebugWindow.AddString("\nPlayer:");
                    DebugWindow.AddString($"Position: {playerPosition}");
                
                    ImGui.GetWindowDrawList().AddCircleFilled(ImGui.GetWindowPos() - viewportPosition + playerPosition + textureSize, 10.0f, ImGui.GetColorU32(Colors.Red));

                    unsafe
                    {
                        var cameraManager = CameraManager.Instance()->CurrentCamera;
                        
                        DebugWindow.AddString("\nCameraInfo:");
                        DebugWindow.AddString($"0: {cameraManager->Vector_0.GetString()}");
                        DebugWindow.AddString($"1: {cameraManager->Vector_1.GetString()}");
                        DebugWindow.AddString($"2: {cameraManager->Vector_2.GetString()}");
                        DebugWindow.AddString($"3: {cameraManager->Vector_3.GetString()}");
                        DebugWindow.AddString($"4: {cameraManager->Vector_4.GetString()}");
                        DebugWindow.AddString($"5: {cameraManager->Vector_5.GetString()}");

                        var cam = *cameraManager;
                        
                        var cvec = ConvertToEuler(cam.Vector_2, cam.Vector_3, cam.Vector_4);

                        var cyaw = -1 * Math.Atan2(-1 * cam.Vector_4.X, -1 * cam.Vector_2.X);
                        var cpitch = -90 * cam.Vector_3.Z;
                        var croll = -1 * Math.Atan2(cam.Vector_3.X, cam.Vector_3.Y) * (180 / Math.PI);

                        var cvecb = new Vector3((float) cyaw, cpitch, (float) croll);


                        var angle = -((float) cyaw - 0.5f * MathF.PI);
                        
                        DebugWindow.AddString($"Angle: {angle}");
                        DebugWindow.AddString($"cvec: {cvec}");
                        DebugWindow.AddString($"cvecb: {cvecb}");
                        DebugWindow.AddString($"yaw: {cyaw}");
                        DebugWindow.AddString($"pitch: {cpitch}");
                        DebugWindow.AddString($"roll: {croll}");

                        var start = ImGui.GetWindowPos() - viewportPosition + playerPosition + textureSize;
                        var lineLength = 30.0f;
                        var stop = new Vector2(lineLength * MathF.Cos(angle), lineLength * MathF.Sin(angle));


                        ImGui.GetWindowDrawList().AddLine(start, start + stop, ImGui.GetColorU32(Colors.Red), 5.0f);
                    }
                }


            }
        }
    }
    
    public static Vector3 ConvertToEuler(Vector3 vecA, Vector3 vecB, Vector3 vecC) 
    {
        var sy = Math.Sqrt(vecA.X * vecA.X + vecB.X * vecB.X);

        var singular = sy < 1e-6;
        
        double x, y, z;

        if (!singular) 
        {
            x = Math.Atan2(-1 * vecC.Y, -1 * vecC.Z) * (180 / Math.PI); 
            y = Math.Atan2(vecC.X, sy) * (180 / Math.PI);
            z = Math.Atan2(-1 * vecB.X, -1 * vecA.X) * (180 / Math.PI);
        } 
        else 
        {
            x = Math.Atan2(vecB.Z, -vecB.Y) * (180 / Math.PI);
            y = Math.Atan2(vecC.X, sy) * (180 / Math.PI);
            z = -255;
        }
        
        return new Vector3((float) x, (float) y, (float) z);
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

public static class Vector3Extensions
{
    public static string GetString(this FFXIVClientStructs.FFXIV.Client.Graphics.Vector3 vector)
    {
        return $"<{vector.X:F5}, {vector.Y:F5}, {vector.Z:F5}>";
    }
}