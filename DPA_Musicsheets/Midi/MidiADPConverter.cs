using DPA_Musicsheets.MusicComponentModels;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Midi
{
    public class MidiADPConverter
    {
        private MidiMusicalSymbolBuilder musicalSymbolBuilder;

        public MidiADPConverter()
        {
            musicalSymbolBuilder = new MidiMusicalSymbolBuilder();
        }

        public ADPSheet convertMidi(String path)
        {
            var sequence = new Sequence();
            sequence.Load(path);

            ADPSheet returnSheet = new ADPSheet();
            ADPTrack tempADPTrack;
            ADPMusicalSymbol tempADPMusicalSymbol;
            int[] timeSignature = new int[2];

            double wholeNoteLength = sequence.Division * 4;


            List<Track> tracks = new List<Track>();

            for (int i = 0; i < sequence.Count; i++)
            {
                tracks.Add(sequence[i]);
            }

            foreach (Track t in tracks)
            {
                tempADPTrack = new ADPTrack();
                ADPBar tempADPBar = new ADPBar();
                tempADPBar.TimeSignature = timeSignature;
                foreach (MidiEvent midiEvent in t.Iterator())
                {
                    // musical symbol
                    if (midiEvent.MidiMessage.MessageType == MessageType.Channel)
                    {
                        var channelMessage = midiEvent.MidiMessage as ChannelMessage;

                        if (channelMessage.Command == ChannelCommand.NoteOn || channelMessage.Command == ChannelCommand.NoteOff)
                        {
                            tempADPMusicalSymbol = musicalSymbolBuilder.BuildMusicalSymbol(channelMessage, midiEvent, wholeNoteLength);
                            if (tempADPMusicalSymbol != null)
                            {
                                tempADPBar.MusicalSymbols.Add(tempADPMusicalSymbol);
                                if(midiEvent.AbsoluteTicks % (wholeNoteLength) == 0)
                                {
                                    //last musical symbol in bar
                                    tempADPTrack.Bars.Add(tempADPBar);
                                    tempADPBar = new ADPBar();
                                    tempADPBar.TimeSignature = timeSignature;
                                }
                            }
                        }
                    }

                    // info over track
                    if (midiEvent.MidiMessage.MessageType == MessageType.Meta)
                    {
                        var metaMessage = midiEvent.MidiMessage as MetaMessage;
                        if (metaMessage.MetaType == MetaType.TrackName)
                        {
                            tempADPTrack.Name = MidiReader.GetMetaString(metaMessage);
                        }
                        if (metaMessage.MetaType == MetaType.TimeSignature)
                        {
                            byte[] bytes = metaMessage.GetBytes();
                            timeSignature[0] = bytes[0];
                            timeSignature[1] = (int)(1 / Math.Pow(bytes[1], -2));
                        }
                    }
                }
                returnSheet.Tracks.Add(tempADPTrack);
            }
            return returnSheet;
        }
    }
}
