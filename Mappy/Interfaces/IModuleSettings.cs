using ImGuiNET;
using Mappy.DataModels;

namespace Mappy.Interfaces;

public interface IModuleSettings
{
    ComponentName ComponentName { get; }
    public bool ShowInConfiguration() => true;

    void DrawLabel()
    {
        ImGui.Text(ComponentName.GetTranslatedString());
    }
    
    void DrawSettings();
}