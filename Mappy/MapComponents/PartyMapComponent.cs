using Mappy.Interfaces;
using Mappy.Utilities;

namespace Mappy.MapComponents;

public class PartyMapComponent : IMapComponent
{
    public void Update(uint mapID)
    {
    }

    public void Draw()
    {
        if (!Service.MapManager.PlayerInCurrentMap) return;

        DrawPlayers();
    }

    private void DrawPlayers()
    {
        foreach (var player in Service.PartyList)
        {
            var playerPosition = Service.MapManager.GetObjectPosition(player.Position);
            var icon = Service.Cache.IconCache.GetIconTexture(60421);

            MapRenderer.DrawIcon(icon, playerPosition, 1.0f);
            MapRenderer.DrawTooltip(player.Name.TextValue, Colors.Blue);
        }
    }
}