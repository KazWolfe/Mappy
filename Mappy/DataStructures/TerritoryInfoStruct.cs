using System.Runtime.InteropServices;
using Lumina.Excel.GeneratedSheets;

namespace Mappy.DataStructures;
/// <summary>
/// 8B 2D ?? ?? ?? ?? 41 BF
/// </summary>
[StructLayout(LayoutKind.Explicit, Size = 76)]
public readonly struct TerritoryInfoStruct
{
    [FieldOffset(8)] private readonly int InSanctuary;
    [FieldOffset(16)] private readonly uint RegionID;
    [FieldOffset(20)] private readonly uint SubAreaID;

    public bool IsInSanctuary => InSanctuary == 1;
    public PlaceName? Region => Service.PlaceNameCache.GetRow(RegionID);
    public PlaceName? SubArea => Service.PlaceNameCache.GetRow(SubAreaID);

}