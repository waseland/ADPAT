using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.MusicComponentModels
{
    public class ADPSheet
    {
        public List<ADPTrack> Tracks { get; set; }

        public ADPSheet()
        {
            Tracks = new List<ADPTrack>();
        }
    }
}
