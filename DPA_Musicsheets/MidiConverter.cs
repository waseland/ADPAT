using DPA_Musicsheets.Models;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets
{
    public class MidiConverter
    {
        private MyMusicalSymbolBuilder noteBuilder;
        private List<Models.MyTrack> Tracks;

        public MidiConverter()
        {
            //trackBuilder = new MyTrackBuilder();
            noteBuilder = new MyMusicalSymbolBuilder();
        }

        public MyMusicSheet convertMidi(String path)
        {
            var sequence = new Sequence();
            sequence.Load(path);
            MyMusicSheet musicSheet = new MyMusicSheet();
            MyTrack tempMyTrack;

            double wholeNoteLength = sequence.Division * 4;
            

            List<Track> tracks = new List<Track>();

            for (int i = 0; i < sequence.Count; i++)
            {
                tracks.Add(sequence[i]);
            }

            foreach (Track i in tracks)
            {
                tempMyTrack = new MyTrack();
                foreach (MidiEvent midiEvent in i.Iterator())
                {
                    //note
                    if (midiEvent.MidiMessage.MessageType == MessageType.Channel)
                    {
                        var channelMessage = midiEvent.MidiMessage as ChannelMessage;

                        if (channelMessage.Command == ChannelCommand.NoteOn || channelMessage.Command == ChannelCommand.NoteOff)
                        {
                            MyMusicalSymbol tempNote = noteBuilder.BuildMusicalSymbol(channelMessage, midiEvent, wholeNoteLength);
                            if(tempNote != null)
                            {
                                tempMyTrack.AddMusicalNote(tempNote);
                            }
                        }
                    }

                    //info over track
                    if (midiEvent.MidiMessage.MessageType == MessageType.Meta)
                    {
                        var metaMessage = midiEvent.MidiMessage as MetaMessage;
                        if (metaMessage.MetaType == MetaType.TrackName)
                        {
                            tempMyTrack.TrackName = MidiReader.GetMetaString(metaMessage);
                        }
                        if (metaMessage.MetaType == MetaType.TimeSignature)
                        {
                            byte[] bytes = metaMessage.GetBytes();
                            musicSheet.TimeSignature[0] = bytes[0];
                            musicSheet.TimeSignature[1] = (int)(1 / Math.Pow(bytes[1], -2));
                        }
                        if (metaMessage.MetaType == MetaType.EndOfTrack)
                        {
                            //string asdf = MidiReader.GetMetaString(metaMessage);
                            //Vraag leraar: endoftrack zou volgens bb absoluteticks moeten hebben maar dat is niet zo
                        }
                    }
                }
                musicSheet.AddTrack(tempMyTrack);
            }
            return musicSheet;
        }

        //public MidiConverter(String path)
        //{
        //    var sequence = new Sequence();
        //    sequence.Load(path);

        //    List<Track> tracks = new List<Track>();

        //    for (int i = 0; i < sequence.Count; i++)
        //    {
        //        tracks.Add(sequence[i]);
        //    }

        //    string trackName = "Error";
        //    int[] timeSignature = new int[2];
        //    string tempo = "Error";
        //    List<Tuple<ChannelMessage, MidiEvent>> notes = new List<Tuple<ChannelMessage, MidiEvent>>();
        //    int ticksPerBeat = sequence.Division;
        //    int endOfTrackAbsoluteTicks = 0;

        //    foreach (Track i in tracks)
        //    {
        //        #region
        //        foreach (MidiEvent midiEvent in i.Iterator())
        //        {
        //            // ChannelMessages zijn de inhoudelijke messages.
        //            if (midiEvent.MidiMessage.MessageType == MessageType.Channel)
        //            {
        //                var channelMessage = midiEvent.MidiMessage as ChannelMessage;

        //                if (channelMessage.Command == ChannelCommand.NoteOn || channelMessage.Command == ChannelCommand.NoteOff)
        //                {
        //                    notes.Add(new Tuple<ChannelMessage, MidiEvent>(channelMessage, midiEvent));
        //                }
        //            }
        //            // Meta zegt iets over de track zelf.
        //            if (midiEvent.MidiMessage.MessageType == MessageType.Meta)
        //            {
        //                var metaMessage = midiEvent.MidiMessage as MetaMessage;
        //                if (metaMessage.MetaType == MetaType.TrackName)
        //                {
        //                    trackName = MidiReader.GetMetaString(metaMessage);
        //                }
        //                if (metaMessage.MetaType == MetaType.Tempo)
        //                {
        //                    tempo = MidiReader.GetMetaString(metaMessage);
        //                }
        //                if (metaMessage.MetaType == MetaType.TimeSignature)
        //                {
        //                    byte[] bytes = metaMessage.GetBytes();
        //                    timeSignature[0] = bytes[0];    //kwart = 1 / 0.25 = 4                   
        //                    timeSignature[1] = (int)(1 / Math.Pow(bytes[1], -2));
        //                }
        //                if (metaMessage.MetaType == MetaType.EndOfTrack)
        //                {
        //                    string asdf = MidiReader.GetMetaString(metaMessage);
        //                    //Vraag leraar: endoftrack zou volgens bb absoluteticks moeten hebben maar dat is niet zo
        //                }
        //            }
        //        }
        //        #endregion
        //    }

        //    trackBuilder.buildMidiToObjectTrack(trackName, timeSignature, tempo, ticksPerBeat, notes);
        //}
    }
}
