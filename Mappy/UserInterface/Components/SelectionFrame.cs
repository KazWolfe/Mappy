using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.UserInterface.Components;

internal class SelectionFrame : IDrawable
{
    public IModuleSettings? Selected { get; private set; }
    private IEnumerable<IModuleSettings> Selectables { get; }
    private readonly IDrawable? extraDrawable;
    private float Weight { get; }
    private readonly string pluginVersion;

    private string searchString = "Search ...";
    
    public SelectionFrame(IEnumerable<IModuleSettings> selectables, float weight = 0.30f, IDrawable? extraDrawable = null)
    {
        Selectables = new List<IModuleSettings>(selectables);
        Weight = weight;
        this.extraDrawable = extraDrawable;

        pluginVersion = GetVersionText();
    }

    public void Draw()
    {
        var regionAvailable = ImGui.GetContentRegionAvail();
        var bottomPadding = (extraDrawable != null ? 50.0f : 25.0f) * ImGuiHelpers.GlobalScale;

        
        if (ImGui.BeginChild("###SelectionFrame", new Vector2(regionAvailable.X * Weight, 0), false))
        {
            ImGui.PushItemWidth(regionAvailable.X * Weight);
            
            ImGui.InputText("###SelectionSearch", ref searchString, 35, ImGuiInputTextFlags.AutoSelectAll);
            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                searchString = "Search ...";
            }
            
            var frameBgColor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
            ImGui.PushStyleColor(ImGuiCol.FrameBg, frameBgColor with { W = 0.05f });

            ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
            if (ImGui.BeginListBox("", new Vector2(-1, -bottomPadding)))
            {
                ImGui.PopStyleColor();

                foreach (var item in Selectables.Where(CompareSearch))
                {
                    ImGui.PushID(item.ComponentName.ToString());

                    var headerHoveredColor = ImGui.GetStyle().Colors[(int)ImGuiCol.HeaderHovered];
                    var textSelectedColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, headerHoveredColor with { W = 0.1f });
                    ImGui.PushStyleColor(ImGuiCol.Header, textSelectedColor with { W = 0.1f });

                    if (ImGui.Selectable("", Selected == item))
                    {
                        Selected = Selected == item ? null : item;
                    }

                    ImGui.PopStyleColor();
                    ImGui.PopStyleColor();

                    ImGui.SameLine(3.0f);

                    item.DrawLabel();

                    ImGui.Spacing();

                    ImGui.PopID();
                }

                ImGui.EndListBox();
            }
            ImGui.PopStyleVar();

            extraDrawable?.Draw();

            DrawVersionText();
        }
        ImGui.EndChild();
        
        ImGui.SameLine();
    }

    private string GetVersionText()
    {
        var assemblyInformation = Assembly.GetExecutingAssembly().FullName!.Split(',');

        var versionString = assemblyInformation[1].Replace('=', ' ');

        var commitInfo = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown";
        return $"{versionString} - {commitInfo}";
    }

    private void DrawVersionText()
    {
        var region = ImGui.GetContentRegionAvail();

        var versionTextSize = ImGui.CalcTextSize(pluginVersion) / 2.0f;
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - versionTextSize.X;

        ImGui.SetCursorPos(cursorStart);
        ImGui.TextColored(Colors.Grey, pluginVersion);
    }

    private bool CompareSearch(IModuleSettings module)
    {
        var searchNormalized = searchString.ToLower();
        var moduleNormalize = module.ComponentName.GetTranslatedString().ToLower();

        return searchNormalized == "search ..." || moduleNormalize.Contains(searchNormalized);
    }
}