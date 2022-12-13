using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Lumina.Excel.GeneratedSheets;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class AllianceMemberSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<uint> SelectedIcon = new(60358);
    public Setting<float> IconScale = new(0.50f);
    public Setting<Vector4> TooltipColor = new(Colors.ForestGreen);
}

public class AllianceMembers : IModule
{
    private static AllianceMemberSettings Settings => Service.Configuration.AllianceSettings;
    public IMapComponent MapComponent { get; } = new AllianceMembersMapComponent();
    public IModuleSettings Options { get; } = new AllianceMemberOptions();

    private class AllianceMembersMapComponent : IMapComponent
    {
        private readonly HashSet<uint> allianceRaidTerritories;
        private bool enableAllianceChecking;
    
        public AllianceMembersMapComponent()
        {
            allianceRaidTerritories = Service.DataManager.GetExcelSheet<TerritoryType>()
                !.Where(r => r.TerritoryIntendedUse is 8)
                .Select(r => r.RowId)
                .ToHashSet();
        }
    
        public void Update(uint mapID)
        {        
            var map = Service.Cache.MapCache.GetRow(mapID);

            enableAllianceChecking = allianceRaidTerritories.Contains(map.TerritoryType.Row);
        }

        public void Draw()
        {
            if (!Settings.Enable.Value) return;
            if (!Service.MapManager.PlayerInCurrentMap) return;
            if (!enableAllianceChecking) return;
        
            DrawAllianceMembers();
        }
    
        private void DrawAllianceMembers()
        {
            if (!enableAllianceChecking) return;
        
            foreach (var index in Enumerable.Range(0, 16))
            {
                var player = HudAgent.GetAllianceMember(index);

                if (player is not null)
                {
                    if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(Settings.SelectedIcon.Value, player, Settings.IconScale.Value);
                    if(Settings.ShowTooltip.Value) MapRenderer.DrawTooltip(player.Name.TextValue, Settings.TooltipColor.Value);
                }
            }
        }
    }

    private class AllianceMemberOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.AllianceMember;
    
        public void DrawSettings()
        {
            InfoBox.Instance
                .AddTitle(Strings.Configuration.FeatureToggles)
                .AddConfigCheckbox(Strings.Map.Generic.Enable, Settings.Enable)
                .AddDummy(8.0f)
                .AddConfigCheckbox(Strings.Map.Generic.ShowIcon, Settings.ShowIcon)
                .AddConfigCheckbox(Strings.Map.Generic.ShowTooltip, Settings.ShowTooltip)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.ColorOptions)
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.ForestGreen)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.IconScale.Value = 0.50f;
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();

            InfoBox.Instance
                .AddTitle(Strings.Configuration.IconSelect)
                .BeginFlexGrid()
                .SingleSelect(Settings.SelectedIcon, 60358, 60359, 60360, 60361)
                .EndFlexGrid()
                .Draw();
        }
    }
}