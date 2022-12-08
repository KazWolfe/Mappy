using System;
using System.Linq;
using Lumina.Excel.GeneratedSheets;

namespace Mappy.System;

public class CompositeLuminaCache : IDisposable
{
    public LuminaCache<PlaceName> PlaceNameCache = new();
    public LuminaCache<Map> MapCache = new();
    public LuminaCache<Aetheryte> AetheryteCache = new();
    public LuminaCache<MapSymbol> MapSymbolCache = new(GetMapSymbol);
    public LuminaCache<GatheringPoint> GatheringPointCache = new();
    public LuminaCache<GatheringPointBase> GatheringPointBaseCache = new();

    public IconManager IconCache = new();
    public MapTextureManager MapTextureCache = new();

    public void Dispose()
    {
        IconCache.Dispose();
        MapTextureCache.Dispose();
    }

    private static MapSymbol GetMapSymbol(uint iconId)
    {
        return Service.DataManager.GetExcelSheet<MapSymbol>()!
            .Where(symbol => symbol.Icon == iconId)
            .First();
    }
}