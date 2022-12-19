using System.Collections.Generic;
using Mappy.DataModels;
using Mappy.Interfaces;

namespace Mappy.Modules;

public class FocusLayer : IModule
{
    public IMapComponent MapComponent { get; } = new FocusLayerMapComponent();
    public IModuleSettings Options { get; } = new FocusLayerOptions();

    public static void AddMarker(IMapMarker marker) => FocusLayerMapComponent.InternalAddMarker(marker);

    public static void AddMarkers(IEnumerable<IMapMarker> markers)
    {
        foreach (var marker in markers)
        {
            AddMarker(marker);
        }
    }
    
    private class FocusLayerMapComponent : IMapComponent
    {
        private static readonly List<IMapMarker> Markers = new();
        
        public void Draw()
        {
            Markers.ForEach(m => m.Draw());
            Markers.Clear();
        }

        public static void InternalAddMarker(IMapMarker marker) => Markers.Add(marker);
    }
    
    private class FocusLayerOptions : IModuleSettings
    {
        public ComponentName ComponentName => ComponentName.Unknown;
        public bool ShowInConfiguration() => false;

        public void DrawSettings()
        {
            // Do Nothing
        }
    }
}