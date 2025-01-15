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
        public TileInformation(TileType _type, string _name, int _cost, Uri _imageLocation)
        {
            Type = _type;
            Name = _name;
            Cost = _cost;
            Location = _imageLocation;
        }

        public TileType Type { get; set; }
        public string Name { get; set; }
        public Uri Location { get; set; }
        public int Cost { get; set; }
    }
}
