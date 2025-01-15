using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public class TileCollection
    {
        public List<TileInformation> Information { get; set; }

        public TileCollection()
        {
            Information = new List<TileInformation>();
        }

        public void AddNewTile(TileInformation tileInformation)
        {
            Information.Add(tileInformation);
        }

        public TileInformation? GetTileInformation(TileType _type)
        {
            TileInformation? tile = null;
            foreach (TileInformation item in Information)
            {
                if(item.Type == _type)
                {
                    tile = item;
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
