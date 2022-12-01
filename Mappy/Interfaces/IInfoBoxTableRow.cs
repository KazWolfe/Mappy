using Mappy.UserInterface.Components;

namespace Mappy.Interfaces;

public interface IInfoBoxTableConfigurationRow
{
    void GetConfigurationRow(InfoBoxTable owner);
}

public interface IInfoBoxTableDataRow
{
    void GetDataRow(InfoBoxTable owner);
}

public interface IInfoBoxListConfigurationRow
{
    void GetConfigurationRow(InfoBoxList owner);
}