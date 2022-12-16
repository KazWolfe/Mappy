using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Localization;
using Mappy.UserInterface.Components;
using Mappy.Utilities;

namespace Mappy.Modules;

public class PetSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<float> IconScale = new(0.75f);
    public Setting<Vector4> TooltipColor = new(Colors.Purple);
}

public class Pets : IModule
{
    private static PetSettings Settings => Service.Configuration.Pet;
    public IMapComponent MapComponent { get; } = new PetMapComponent();
    public IModuleSettings Options { get; } = new PetOptions();
    private class PetMapComponent : IMapComponent
    {
        public void Draw()
        {
            if (!Settings.Enable.Value) return;
            if (!Service.MapManager.PlayerInCurrentMap) return;

            DrawPets();
        }
    
        private void DrawPets()
        {
            if (Service.PartyList.Length == 0)
            {
                if (Service.ClientState.LocalPlayer is { } localPlayer)
                {
                    DrawPet(localPlayer.ObjectId);
                }
            }
            else
            {
                foreach (var partyMember in Service.PartyList)
                {
                    DrawPet(partyMember.ObjectId);
                }
            }
        }
    
        private void DrawPet(uint ownerID)
        {
            foreach (var obj in OwnedPets(ownerID))
            {
                if(Settings.ShowIcon.Value) MapRenderer.DrawIcon(60961, obj, Settings.IconScale.Value);
                if(Settings.ShowTooltip.Value) MapRenderer.DrawTooltip(obj.Name.TextValue, Settings.TooltipColor.Value);
            }
        }
    
        private IEnumerable<GameObject> OwnedPets(uint objectID)
        {
            var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == objectID);
        
            return ownedObjects.Where(obj => obj.ObjectKind == ObjectKind.BattleNpc && IsPetOrChocobo(obj));
        }

        private static bool IsPetOrChocobo(GameObject gameObject)
        {
            var battleNpc = gameObject as BattleNpc;

            return  (BattleNpcSubKind?)battleNpc?.SubKind switch
            {
                BattleNpcSubKind.Chocobo => true,
                BattleNpcSubKind.Enemy => false,
                BattleNpcSubKind.None => false,
                BattleNpcSubKind.Pet => true,
                _ => false
            };
        }
    }
    private class PetOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.Pet;
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
                .AddConfigColor(Strings.Map.Generic.TooltipColor, Settings.TooltipColor, Colors.Purple)
                .Draw();
        
            InfoBox.Instance
                .AddTitle(Strings.Configuration.Adjustments)
                .AddDragFloat(Strings.Map.Generic.IconScale, Settings.IconScale, 0.10f, 5.0f, InfoBox.Instance.InnerWidth / 2.0f)
                .AddButton(Strings.Configuration.Reset, () =>
                {
                    Settings.IconScale.Value = 0.75f;
                    Service.Configuration.Save();
                }, new Vector2(InfoBox.Instance.InnerWidth, 23.0f * ImGuiHelpers.GlobalScale))
                .Draw();
        }
    }
}

