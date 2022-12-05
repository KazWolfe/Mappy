using System;
using System.Linq;
using Lumina.Excel.GeneratedSheets;

namespace Mappy.System;

public class CompositeLuminaCache : IDisposable
{
    public LuminaCache<PlaceName> PlaceNameCache = new();
    public LuminaCache<Map> MapCache = new();
    public LuminaCache<Aetheryte> AetheryteCache = new();
    public LuminaCache<MapMarker> MapMarkerCache = new();
    public LuminaCache<MapSymbol> MapSymbolCache = new(GetMapSymbol);

    public IconManager IconCache = new();

    public void Dispose()
    {
        IconCache.Dispose();
    }

    private static MapSymbol GetMapSymbol(uint iconId)
    {
        return Service.DataManager.GetExcelSheet<MapSymbol>()!
            .Where(symbol => symbol.Icon == iconId)
            .First();
    }
}