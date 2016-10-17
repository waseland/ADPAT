using System.Collections.Generic;

namespace DPA_Musicsheets.MusicComponentModels
{
    public class ADPSheet
    {
        public List<ADPTrack> Tracks { get; set; }

        public ADPSheet()
        {
            Tracks = new List<ADPTrack>();
        }

        public ADPTrack getTrack()
        {
            foreach(ADPTrack track in Tracks)
            {
                if(track.Bars.Count > 0)
                {
                    return track;
                }
            }
            return Tracks[0];
        }
    }
}
