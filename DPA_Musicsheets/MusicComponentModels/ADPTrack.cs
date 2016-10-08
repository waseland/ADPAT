using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.MusicComponentModels
{
    public class ADPTrack
    {
        public List<ADPBar> Bars { get; set; }
        public string Name { get; set; }

        public ADPTrack()
        {
            Bars = new List<ADPBar>();
        }
    }
}
