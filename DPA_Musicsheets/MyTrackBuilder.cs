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

        public void buildMidiToObjectTrack(string trackName, int[] timeSignature, string tempo, int ticksperBeat, List<Tuple<ChannelMessage, MidiEvent>> notes)
        {
            MyTrack track = new MyTrack();
            track.TrackName = trackName;
            track.TimeSignature = timeSignature;
            track.Tempo = Int32.Parse(tempo.Substring(7));
            track.TicksPerBeat = ticksperBeat;

            foreach (Tuple<ChannelMessage, MidiEvent> c in notes)
            {
                track.AddNote(c.Item1, c.Item2);
            }

            track.SetNoteDuration();
            tracks.Add(track);
        }

        public void buildLyToObjectTrack()
        {

        }
    }
}
