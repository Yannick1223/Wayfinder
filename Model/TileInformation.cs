using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
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
