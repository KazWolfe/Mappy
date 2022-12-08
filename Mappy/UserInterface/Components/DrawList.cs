using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using Mappy.DataModels;
using Mappy.Localization;

namespace Mappy.UserInterface.Components;

public abstract class DrawList<T>
{
    protected T DrawListOwner { get; init; } = default!;
    protected List<Action> DrawActions { get; } = new();

    protected void DrawListContents()
    {
        foreach (var action in DrawActions)
        {
            action();
        }
    }

    public T AddIndent(int tab)
    {
        DrawActions.Add(() => ImGui.Indent(15.0f * tab));

        return DrawListOwner;
    }

    public T AddString(string message, Vector4? color = null)
    {
        if (color == null)
        {
            DrawActions.Add(() => ImGui.Text(message));
        }
        else
        {
            DrawActions.Add(() => ImGui.TextColored(color.Value, message));
        }

        return DrawListOwner;
    }

    public T AddConfigCheckbox(string label, Setting<bool> setting, string? helpText = null, string? additionalID = null)
    {
        DrawActions.Add(() =>
        {
            if (additionalID != null)
            {
                ImGui.PushID(additionalID);
            }
            
            var cursorPosition = ImGui.GetCursorPos();

            if (ImGui.Checkbox($"##{label}", ref setting.Value))
            {
                Service.Configuration.Save();
            }

            var spacing = ImGui.GetStyle().ItemSpacing;
            cursorPosition += spacing;
            ImGui.SetCursorPos(cursorPosition with { X = cursorPosition.X + 27.0f * ImGuiHelpers.GlobalScale});

            ImGui.TextUnformatted(label);

            if (helpText != null)
            {
                ImGuiComponents.HelpMarker(helpText);
            }

            if (additionalID != null)
            {
                ImGui.PopID();
            }
        });

        return DrawListOwner;
    }

    public T AddConfigCombo<TU>(IEnumerable<TU> values, Setting<TU> setting, Func<TU, string> localizeFunction, string label = "", float width = 0.0f) where TU : struct
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            if (ImGui.BeginCombo(label, localizeFunction(setting.Value)))
            {
                foreach (var value in values)
                {
                    if (ImGui.Selectable(localizeFunction(value), setting.Value.Equals(value)))
                    {
                        setting.Value = value;
                        Service.Configuration.Save();
                    }
                }

                ImGui.EndCombo();
            }
        });

        return DrawListOwner;
    }

    public T AddConfigColor(string label, Setting<Vector4> setting)
    {
        DrawActions.Add(() =>
        {
            if (ImGui.ColorEdit4(label, ref setting.Value, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf))
            {
                Service.Configuration.Save();
            }
        });

        return DrawListOwner;
    }
    
    public T AddConfigColor(string label, Setting<Vector4> setting, Vector4 defaultValue)
    {
        DrawActions.Add(() =>
        {
            ImGui.PushID(label);
            
            if (ImGui.ColorEdit4($"##{label}", ref setting.Value, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf))
            {
                Service.Configuration.Save();
            }
            
            ImGui.SameLine();
            ImGui.BeginDisabled(setting.Value == defaultValue);
            if (ImGui.Button(Strings.Configuration.Default))
            {
                setting.Value = defaultValue;
                Service.Configuration.Save();
            }
            ImGui.EndDisabled();
            
            ImGui.SameLine();
            ImGui.TextUnformatted(label);
            
            ImGui.PopID();
        });

        return DrawListOwner;
    }

    public T AddDragFloat(string label, Setting<float> setting, float minValue, float maxValue, float width = 0.0f)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.DragFloat(label, ref setting.Value, 0.01f * maxValue, minValue, maxValue, "%.2f");
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.Configuration.Save();
            }
        });

        return DrawListOwner;
    }

    public T AddAction(Action action)
    {
        DrawActions.Add(action);

        return DrawListOwner;
    }
    
    public T AddSliderInt(string label, Setting<int> setting, int minValue, int maxValue, float width = 200.0f)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.SliderInt(label, ref setting.Value, minValue, maxValue);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.Configuration.Save();
            }
        });

        return DrawListOwner;
    }
    
    public T AddIconImage(uint iconId)
    {
        DrawActions.Add(() =>
        {
            if (Service.Cache.IconCache.GetIconTexture(iconId) is { } icon)
            {
                var iconSize = new Vector2(icon.Width, icon.Height);
            
                ImGui.Image(icon.ImGuiHandle, iconSize);
            }
        });

        return DrawListOwner;
    }

    public T AddConfigRadio<TU>(string label, Setting<TU> setting, TU buttonValue, string? helpText = null ) where TU : struct
    {
        DrawActions.Add(() =>
        {
            var value = Convert.ToInt32(setting.Value);

            if (ImGui.RadioButton(label, ref value, Convert.ToInt32(buttonValue)))
            {
                setting.Value = (TU)Enum.ToObject(typeof(TU), value);
                Service.Configuration.Save();
            }

            if (helpText != null)
            {
                ImGuiComponents.HelpMarker(helpText);
            }
        });

        return DrawListOwner;
    }

    public T AddConfigIconRadio(string label, Setting<uint> setting, uint iconValue, string? helpText = null)
    {
        DrawActions.Add(() =>
        {
            var value = Convert.ToInt32(setting.Value);

            var cursorPosition = ImGui.GetCursorPos();
            if (Service.Cache.IconCache.GetIconTexture(iconValue) is { } icon)
            {
                var iconSize = new Vector2(icon.Width, icon.Height) / 2.0f;
                
                ImGui.SetCursorPos(cursorPosition with { Y = cursorPosition.Y + iconSize.Y / 2.0f });
                if (ImGui.RadioButton($"##{label}", ref value, (int)iconValue))
                {
                    setting.Value = (uint)value;
                    Service.Configuration.Save();
                }

                ImGui.SetCursorPos(cursorPosition with { X = cursorPosition.X + 20.0f * ImGuiHelpers.GlobalScale});
                ImGui.Image(icon.ImGuiHandle, new Vector2(icon.Width, icon.Height));
            }
            if (helpText != null)
            {
                ImGuiComponents.HelpMarker(helpText);
            }
        });

        return DrawListOwner;
    }

    public T AddConfigString(Setting<string> settingsCustomName, float width = 0.0f)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }

            ImGui.InputText("", ref settingsCustomName.Value, 24);

            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.Configuration.Save();
            }
        });

        return DrawListOwner;
    }

    public T AddConfigVector2(Setting<Vector2> setting, float width = 200.0f)
    {
        DrawActions.Add(() =>
        {
            if (width != 0.0f)
            {
                ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            }
            else
            {
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
            }

            ImGui.InputFloat2("", ref setting.Value);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                Service.Configuration.Save();
            }
        });

        return DrawListOwner;
    }

    public T AddInputInt(string label, Setting<int> settingsPriority, int min, int max, int step = 1, int stepFast = 1, float width = 77.0f)
    {
        DrawActions.Add(() =>
        {
            ImGui.SetNextItemWidth(width * ImGuiHelpers.GlobalScale);
            ImGui.InputInt(label, ref settingsPriority.Value, step, stepFast);
            if (ImGui.IsItemDeactivatedAfterEdit())
            {
                settingsPriority.Value = Math.Clamp(settingsPriority.Value, min, max);
                Service.Configuration.Save();
            }
        });

        return DrawListOwner;
    }

    public T SameLine(float width = 0)
    {
        if (width == 0)
        {
            DrawActions.Add(ImGui.SameLine);
        }
        else
        {
            DrawActions.Add(() => ImGui.SameLine(width));
        }

        return DrawListOwner;
    }

    public T AddButton(string label, Action action, Vector2? buttonSize = null)
    {
        if (buttonSize is not null)
        {
            DrawActions.Add(() =>
            {
                if (ImGui.Button(label, buttonSize.Value))
                {
                    action.Invoke();
                }
            });
        }
        else
        {
            DrawActions.Add(() =>
            {
                if (ImGui.Button(label))
                {
                    action.Invoke();
                }
            });
        }

        return DrawListOwner;
    }
}