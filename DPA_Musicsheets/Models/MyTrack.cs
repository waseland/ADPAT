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
        public int[] TimeSignature { get; set; }
        public int Tempo { get; set; }
        public int TicksPerBeat { get; set; }

        public List<MyNote> notes;

        public MyTrack()
        {
            notes = new List<MyNote>();
        }

        public void AddNote(ChannelMessage message, MidiEvent midiEvent)
        {
            MyNote note = new MyNote(message.Data1, midiEvent.AbsoluteTicks);
            notes.Add(note);
        }

        public void SetNoteDuration()
        {
            for (int i = 0; i < notes.Count - 1; i++)
            {
                notes[i].Type = ((double)notes[i + 1].AbsoluteTicks - (double)notes[i].AbsoluteTicks) / (double)TicksPerBeat;
            }
        }
    }
}
