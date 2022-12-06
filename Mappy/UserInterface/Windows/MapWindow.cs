using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using Mappy.Localization;
using Mappy.System;
using Mappy.Utilities;
using Condition = Mappy.Utilities.Condition;

namespace Mappy.UserInterface.Windows;

public class MapWindow : Window, IDisposable
{
    private IEnumerable<SearchResult>? searchResults;
    private bool shouldFocusMapSearch;
    private bool showMapSelectOverlay;
    private Vector2 mouseDragStart;
    private bool dragStarted;
    private Vector2 lastWindowSize = Vector2.Zero;
    private string searchString = "Search...";
    
    public MapWindow() : base("Mappy Map Window")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(410,200),
            MaximumSize = new Vector2(9999,9999)
        };

        IsOpen = Service.Configuration.KeepOpen.Value;
    }

    public void Dispose()
    {
    }

    public override void PreOpenCheck()
    {
        if (Service.Configuration.KeepOpen.Value) IsOpen = true;
        
        if (Service.ClientState.IsPvP) IsOpen = false;
        if (!Service.ClientState.IsLoggedIn) IsOpen = false;
        if (Condition.BetweenAreas()) IsOpen = false;
        if (Service.Configuration.HideInDuties.Value && Condition.IsBoundByDuty()) IsOpen = false; 
    }
    
    public override void Draw()
    {
        SetFlags();
        if (!showMapSelectOverlay) ReadMouse();

        if (ImGui.BeginChild("###MapFrame", ImGui.GetContentRegionAvail(), false, Flags))
        {
            var shouldFadeMap = Service.Configuration.FadeWhenUnfocused.Value && !IsFocused;
            MapRenderer.DrawMap(shouldFadeMap);

            foreach (var component in Service.MapManager.MapComponents.TakeWhile(_ => !Service.MapManager.LoadingNextMap && !showMapSelectOverlay))
            {
                component.Draw();
            }

            if (IsFocused) DrawToolbar();
            if (!IsFocused) showMapSelectOverlay = false;
            if (showMapSelectOverlay) DrawMapSelect();
        }
        ImGui.EndChild();
    }

    private void DrawMapSelect()
    {
        var searchWidth = 250.0f * ImGuiHelpers.GlobalScale;
        
        var drawStart = ImGui.GetWindowPos();
        var drawStop = drawStart + ImGui.GetWindowSize();
        var backgroundColor = ImGui.GetColorU32(Vector4.Zero with { W = 0.8f });
        
        ImGui.GetWindowDrawList().AddRectFilled(drawStart, drawStop, backgroundColor);

        var regionAvailable = ImGui.GetContentRegionAvail();
        var searchPosition = regionAvailable with {X = regionAvailable.X / 2.0f, Y = regionAvailable.Y / 4.0f};
        ImGui.SetCursorPos(searchPosition - new Vector2(searchWidth / 2.0f, 0.0f));
        ImGui.PushItemWidth(searchWidth);
        
        if (shouldFocusMapSearch)
        {
            ImGui.SetKeyboardFocusHere();
            shouldFocusMapSearch = false;
        }
        
        if (ImGui.InputText("###MapSearch", ref searchString, 60, ImGuiInputTextFlags.AutoSelectAll))
        {
            searchResults = MapSearch.Search(searchString, 10);
        }

        ImGui.SetCursorPos(searchPosition - new Vector2(searchWidth / 2.0f, 0.0f) + ImGuiHelpers.ScaledVector2(0.0f, 30.0f));
        if (ImGui.BeginChild("###SearchResultsChild", new Vector2(searchWidth, regionAvailable.Y * 3.0f / 4.0f )))
        {
            if (searchResults is not null)
            {
                foreach (var result in searchResults)
                {
                    ImGui.SetNextItemWidth(250.0f * ImGuiHelpers.GlobalScale);
                    if (ImGui.Selectable(result.Label))
                    {
                        Service.Configuration.FollowPlayer.Value = false;
                        Service.MapManager.LoadMap(result.MapID);
                        showMapSelectOverlay = false;
                    }
                }
            }
        }
        ImGui.EndChild();
    }

    private void DrawToolbar()
    {
        var regionAvailable = ImGui.GetContentRegionAvail();
        ImGui.SetCursorPos(regionAvailable with {Y = 0, X = 0});

        ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0.0f, 0.0f, 0.0f, 0.80f));        
        if (ImGui.BeginChild("###Toolbar", regionAvailable with { Y = 40.0f }, true))
        {
            DrawMapLayersWidget();
            ImGui.SameLine();
            DrawFollowPlayerWidget();
            ImGui.SameLine();
            DrawRecenterOnPlayerWidget();
            ImGui.SameLine();
            DrawFindMapWidget();
            ImGui.SameLine();
            DrawConfigurationButton();
        }
        ImGui.EndChild();
        ImGui.PopStyleColor();
    }

    private void DrawConfigurationButton()
    {
        ImGui.PushID("ConfigurationButton");
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Cog.ToIconString(), new Vector2(25.0f, 23.0f)))
        {
            if (Service.WindowManager.GetWindowOfType<ConfigurationWindow>(out var configurationWindow))
            {
                configurationWindow.IsOpen = !configurationWindow.IsOpen;
                configurationWindow.Collapsed = false;
            }
        }

        ImGui.PopFont();
        ImGui.PopID();
    }

    private void DrawFindMapWidget()
    {
        ImGui.PushID("FindMapWidget");
        ImGui.PushFont(UiBuilder.IconFont);

        var showOverlay = showMapSelectOverlay;

        if (showOverlay) ImGui.PushStyleColor(ImGuiCol.Button, Colors.Red);
        if (ImGui.Button(FontAwesomeIcon.Map.ToIconString(), new Vector2(26.0f, 23.0f)))
        {
            showMapSelectOverlay = !showMapSelectOverlay;
            shouldFocusMapSearch = true;
        }
        if (showOverlay) ImGui.PopStyleColor();

        ImGui.PopFont();
        ImGui.PopID();
    }
    
    private void DrawMapLayersWidget()
    {
        ImGui.PushItemWidth(250.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.BeginCombo("###LayerCombo", Service.MapManager.Map?.GetName() ?? "Unable To Get Map Data"))
        {
            var mapLayers = Service.MapManager.MapLayers;
            
            if (mapLayers.Count == 0)
            {
                ImGui.TextColored(Colors.Orange, Strings.Map.NoLayers);
            }
            else
            {
                foreach (var layer in mapLayers)
                {
                    var subAreaName = layer.GetSubName();
                    
                    if(subAreaName == string.Empty) continue;

                    if (ImGui.Selectable(subAreaName))
                    {
                        Service.Configuration.FollowPlayer.Value = false;
                        Service.MapManager.LoadMap(layer.RowId);
                    }
                }
            }
            ImGui.EndCombo();
        }
    }

    private void DrawFollowPlayerWidget()
    {
        ImGui.PushID("FollowPlayerButton");
        ImGui.PushFont(UiBuilder.IconFont);

        var followPlayer = Service.Configuration.FollowPlayer.Value;

        if (followPlayer) ImGui.PushStyleColor(ImGuiCol.Button, Colors.Red);
        if (ImGui.Button(FontAwesomeIcon.MapMarkerAlt.ToIconString(), new Vector2(23.0f)))
        {
            Service.Configuration.FollowPlayer.Value = !Service.Configuration.FollowPlayer.Value;
        }

        if (followPlayer) ImGui.PopStyleColor();

        ImGui.PopFont();
        ImGui.PopID();
    }

    private void DrawRecenterOnPlayerWidget()
    {
        ImGui.PushID("CenterOnPlayer");
        ImGui.PushFont(UiBuilder.IconFont);

        if (ImGui.Button(FontAwesomeIcon.Crosshairs.ToIconString(), new Vector2(23.0f)))
        {
            MapManager.CenterOnPlayer();
        }

        ImGui.PopFont();
        ImGui.PopID();
    }

    private void ReadMouse()
    {
        if (!ImGui.IsWindowFocused(ImGuiFocusedFlags.AnyWindow) || IsFocused)
        {
            if (IsCursorInWindow() && !IsCursorInWindowHeader())
            {
                if (ImGui.GetIO().MouseWheel > 0) // Mouse Wheel Up
                {
                    MapRenderer.ZoomIn(0.2f);
                }
                else if (ImGui.GetIO().MouseWheel < 0) // Mouse Wheel Down
                {
                    MapRenderer.ZoomOut(0.2f);
                }

                // Don't allow a drag to start if the window size is changing
                if (ImGui.GetWindowSize() == lastWindowSize)
                {
                    if (ImGui.IsMouseDown(ImGuiMouseButton.Left) && !dragStarted)
                    {
                        mouseDragStart = ImGui.GetMousePos();
                        dragStarted = true;
                    }
                }
                else
                {
                    lastWindowSize = ImGui.GetWindowSize();
                    dragStarted = false;
                }
            }

            if (ImGui.IsMouseDragging(ImGuiMouseButton.Left) && dragStarted)
            {
                var delta = mouseDragStart - ImGui.GetMousePos();
                MapRenderer.MoveViewportCenter(delta);
                mouseDragStart = ImGui.GetMousePos();
            }
            else
            {
                dragStarted = false;
            }

            if (dragStarted)
            {
                Service.Configuration.FollowPlayer.Value = false;
            }
        }
    }
    private void SetFlags()
    {
        Flags = WindowFlags.AlwaysActiveFlags;

        if (Service.Configuration.LockWindow.Value) Flags |= WindowFlags.NoMoveResizeFlags;
        if (Service.Configuration.HideWindowFrame.Value) Flags |= WindowFlags.NoDecorationFlags;
        if (!IsCursorInWindowHeader()) Flags |= ImGuiWindowFlags.NoMove;
    }
    private static bool IsCursorInWindow()
    {
        var windowStart = ImGui.GetWindowPos();
        var windowSize = ImGui.GetWindowSize();

        return IsBoundedBy(ImGui.GetMousePos(), windowStart, windowStart + windowSize);
    }
    private static bool IsCursorInWindowHeader()
    {
        var windowStart = ImGui.GetWindowPos();
        var headerSize = ImGui.GetWindowSize() with { Y = ImGui.GetWindowContentRegionMin().Y };
        
        return IsBoundedBy(ImGui.GetMousePos(), windowStart, windowStart + headerSize);
    }
    private static bool IsBoundedBy(Vector2 cursor, Vector2 minBounds, Vector2 maxBounds)
    {
        if (cursor.X >= minBounds.X && cursor.Y >= minBounds.Y)
        {
            if (cursor.X <= maxBounds.X && cursor.Y <= maxBounds.Y)
            {
                return true;
            }
        }

        return false;
    }
}