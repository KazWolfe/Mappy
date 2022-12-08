using ImGuiNET;
using Mappy.DataModels;

namespace Mappy.Interfaces;

public interface IModuleSettings
{
    ComponentName ComponentName { get; }

    void DrawLabel()
    {
        ImGui.Text(ComponentName.GetTranslatedString());
    }
    
    void Draw();
}