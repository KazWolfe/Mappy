using Mappy.DataModels;

namespace Mappy.Interfaces;

public interface IMapComponent
{
    MapData MapData { get; }
    
    void Draw();
    void Refresh();
}