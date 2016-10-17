using System.Collections.Generic;

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
