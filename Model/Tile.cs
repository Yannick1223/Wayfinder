using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public enum TileType
    {
        Start, // Cost = 1
        End, // Cost = 1
        Land,  // Cost = 1
        Desert, // Cost 2
        Water, // Cost = Infinity
        Forest, //Cost = 3
    }

    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
        public int Cost { get; set; }


        public Tile()
        {

        }
    }
}
