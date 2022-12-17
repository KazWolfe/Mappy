using System;
using System.Collections.Generic;
using System.Linq;
using Mappy.Interfaces;
using Mappy.Modules;

namespace Mappy.System;

public class ModuleManager : IDisposable
{
    private readonly List<IModule> modules = new()
    {
        new Fates(),
        new Houses(),
        new MapMarkers(),
        new GatheringPoints(),
        new Quests(),
        new Treasures(),
        new AllianceMembers(),
        new Pets(),
        new PartyMembers(),
        new Waymarks(),
        new GatheringAreaMarker(),
        new FlagMarker(),
        
        new Player() // Always Keep Player Last
    };

    public IEnumerable<IMapComponent> GetMapComponents() => modules.Select(module => module.MapComponent);

    public IEnumerable<IModuleSettings> GetModuleSettings() => modules.Select(module => module.Options).Reverse();

    public void Dispose()
    {
        // No Modules Implement Dispose
    }
}