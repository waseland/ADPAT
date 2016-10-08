using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Models
{
    public class MyTrack
    {
        public string TrackName { get; set; }
        //public int[] TimeSignature { get; set; }
        public int Tempo { get; set; }
        public int TicksPerBeat { get; set; }

        public List<MyMusicalSymbol> Notes;

        public MyTrack()
        {
            //TimeSignature = new int[2];
            Notes = new List<MyMusicalSymbol>();
        }

        public void AddMusicalNote(MyMusicalSymbol _musicalSymbol)
        {
            Notes.Add(_musicalSymbol);
        }

        public void SetNoteDuration()
        {
            for (int i = 0; i < Notes.Count - 1; i++)
            {
                Notes[i].Type = ((double)Notes[i + 1].AbsoluteTicks - (double)Notes[i].AbsoluteTicks) / (double)TicksPerBeat;
            }
        }
    }
}
