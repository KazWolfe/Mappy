using ImGuiNET;
using Mappy.DataModels;

namespace Mappy.Interfaces;

public interface ISelectable
{
    ComponentName ComponentName { get; }

    void DrawLabel()
    {
        ImGui.Text(ComponentName.GetTranslatedString());
    }
    
    void Draw();
}