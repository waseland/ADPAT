using DPA_Musicsheets.MusicComponentModels;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPA_Musicsheets.Midi
{
    public class MidiADPConverter : ADPFileConverter
    {
        private MidiMusicalSymbolBuilder musicalSymbolBuilder;

        public MidiADPConverter()
        {
            musicalSymbolBuilder = new MidiMusicalSymbolBuilder();
            ext = ".mid";
        }

        public override ADPSheet ReadFile(String path)
        {
            var sequence = new Sequence();
            sequence.Load(path);

            ADPSheet returnSheet = new ADPSheet();
            ADPTrack tempADPTrack;
            ADPMusicalSymbol tempADPMusicalSymbol;
            int[] timeSignature = new int[2];
            bool timeSignatureIsSet = false;

            double wholeNoteLength = sequence.Division * 4;
            int barAbsoluteTime = (int)wholeNoteLength;


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
                                if(midiEvent.AbsoluteTicks % (barAbsoluteTime) == 0)
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
                            if(!timeSignatureIsSet)
                            {
                                byte[] bytes = metaMessage.GetBytes();
                                timeSignature[0] = bytes[0];
                                timeSignature[1] = (int)Math.Pow(2, bytes[1]);

                                barAbsoluteTime = calculateBarLength(timeSignature, wholeNoteLength);

                                timeSignatureIsSet = true;
                            }
                        }
                    }
                }
                returnSheet.Tracks.Add(tempADPTrack);
            }
            return returnSheet;
        }

        private int calculateBarLength(int[] timeSignature, double wholeNoteLength)
        {
            double beatNoteLength = wholeNoteLength / timeSignature[1];
            int barLength = (int)(timeSignature[0] * beatNoteLength);
            return barLength;
        }
    }
}
