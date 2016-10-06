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

        public MyMusicSheet()
        {
            Tracks = new List<MyTrack>();
        }

        public void AddTrack(MyTrack newTrack)
        {
            Tracks.Add(newTrack);
        }
    }
}
