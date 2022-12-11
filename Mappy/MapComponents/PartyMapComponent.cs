using System.Numerics;
using Mappy.DataModels;
using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PartyMemberSettings
{
    public Setting<bool> Enable = new(true);
    public Setting<bool> ShowIcon = new(true);
    public Setting<bool> ShowTooltip = new(true);
    public Setting<float> IconScale = new(0.75f);
    public Setting<Vector4> TooltipColor = new(Colors.Blue);
}

public class PartyMemberMapComponent : IMapComponent
{
    private static PartyMemberSettings Settings => Service.Configuration.PartyMembers;
    
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