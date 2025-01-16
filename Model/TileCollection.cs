using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public class TileCollection
    {
        public List<TileInformation> Information { get; private set; }

        public TileCollection()
        {
            Information = new List<TileInformation>();
        }

        public void AddTile(TileInformation _tileInformation)
        {
            Information.Add(_tileInformation);
        }

        public void AddTiles(TileInformation[] _tilesInformation)
        {
            Information.AddRange(_tilesInformation);
        }

        public TileInformation? GetTileInformation(TileType _type)
        {
            TileInformation? tile = null;
            foreach (TileInformation info in Information)
            {
                if(info.Type.Equals(_type))
                {
                    tile = info;
                    break;
                }
            }

            return tile;
        }

        public string? GetTileName(TileType _type)
        {
            return GetTileInformation(_type)?.Name;
        }

        public Uri? GetTileLocation(TileType _type)
        {
            return GetTileInformation(_type)?.Location;
        }

        public int? GetCost(TileType _type)
        {
            return GetTileInformation(_type)?.Cost;
        }
    }
}
