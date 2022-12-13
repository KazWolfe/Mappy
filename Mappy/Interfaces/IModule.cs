namespace Mappy.Interfaces;

public interface IModule
{
    IMapComponent MapComponent { get; }
    IModuleSettings Options { get; }
}