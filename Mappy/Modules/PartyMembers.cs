using System.Numerics;
using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class PartyMemberSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<float> IconScale = new(0.75f);
    public Setting<Vector4> TooltipColor = new(Colors.Blue);
}

public class PartyMembers : IModule
{
    private static PartyMemberSettings Settings => Service.Configuration.PartyMembers;
    public IMapComponent MapComponent { get; } = new PartyMemberMapComponent();
    public IModuleSettings Options { get; } = new PartyMemberOptions();
    private class PartyMemberMapComponent : IMapComponent
    {
    
        public void Update(uint mapID)
        {
        }

        public void Draw()
        {
            if (!Settings.Enable.Value) return;
            if (!Service.MapManager.PlayerInCurrentMap) return;

            DrawPlayers();
        }

        private void DrawPlayers()
        {
            foreach (var player in Service.PartyList)
            {
                if(player.ObjectId == Service.ClientState.LocalPlayer?.ObjectId) continue;
            
                var playerPosition = Service.MapManager.GetObjectPosition(player.Position);

                if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(60421, playerPosition, Settings.IconScale.Value);
                if(Settings.ShowTooltip.Value) MapRenderer.DrawTooltip(player.Name.TextValue, Settings.TooltipColor.Value);
            }
        }
    }
    private class PartyMemberOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.PartyMember;
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
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.Blue)
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
        }
    }
}

