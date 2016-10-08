using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class MyMusicSheet
    {
        public List<MyTrack> Tracks { get; private set; }
        public int[] TimeSignature { get; set; }

        public MyMusicSheet()
        {
            TimeSignature = new int[2];
            Tracks = new List<MyTrack>();
        }

        public void AddTrack(MyTrack newTrack)
        {
            Tracks.Add(newTrack);
        }
    }
}
