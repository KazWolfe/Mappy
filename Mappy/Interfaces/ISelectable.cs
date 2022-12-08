using Mappy.DataModels;

namespace Mappy.Interfaces;

public interface ISelectable
{
    ComponentName ComponentName { get; }
    void DrawLabel();
    void Draw();
}