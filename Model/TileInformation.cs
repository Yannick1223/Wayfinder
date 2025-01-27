using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public enum TileType
    {
        Start, //Cost = 1
        End, //Cost = 1
        Land,  //Cost = 1
        Desert, //Cost 2
        Water, //Cost = Infinity
        Forest, //Cost = 3
        Snow, //Cost = 2
        Bridge_Vertical, //Cost = 1
        Bridge_Horizontal, //Cost = 1
    }

    public class TileInformation: ObservableObject
    {
        public TileType Type { get; private set; }
        public string Name { get; private set; }
        public Uri Location { get; private set; }
        public int Cost { get; private set; }

        public TileInformation(TileType _type, string _name, int _cost, Uri _imageLocation)
        {
            Type = _type;
            Name = _name;
            Cost = _cost;
            Location = _imageLocation;
        }
    }
}
