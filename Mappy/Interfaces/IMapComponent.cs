namespace Mappy.Interfaces;

public interface IMapComponent
{
    void Update(uint mapID)
    {
        // Do Nothing, Most Components won't need this
    }
    
    void Draw();
}