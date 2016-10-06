using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets
{
    public class MyTrackBuilder
    {
        public List<MyTrack> tracks = new List<MyTrack>();

        public void buildMidiToObjectTrack(string trackName, int[] timeSignature, string tempo, int ticksperBeat)
        {
            MyTrack track = new MyTrack();
            track.TrackName = trackName;
            //track.TimeSignature = timeSignature;
            track.Tempo = Int32.Parse(tempo.Substring(7));
            track.TicksPerBeat = ticksperBeat;

            track.SetNoteDuration();
            tracks.Add(track);
        }
    }
}
