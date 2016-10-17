using DPA_Musicsheets.MusicComponentModels;
using Sanford.Multimedia.Midi;
using System;
using System.Collections.Generic;

namespace DPA_Musicsheets.Midi
{
    public class MidiADPConverter : ADPFileConverter
    {
        private List<string> keyValues;

        public MidiADPConverter()
        {
            InitKeyValues();
            ext = ".mid";
        }

        public override ADPSheet ReadFile(String _path)
        {
            var sequence = new Sequence();
            sequence.Load(_path);

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
                            string[] inputStrings = convertToInputStrings(channelMessage, midiEvent, wholeNoteLength);
                            if (inputStrings != null)
                            {
                                tempADPMusicalSymbol = musicalSymbolFactory.GetMusicalSymbol(inputStrings);
                                if (tempADPMusicalSymbol != null)
                                {
                                    tempADPBar.MusicalSymbols.Add(tempADPMusicalSymbol);
                                    if (midiEvent.AbsoluteTicks % (barAbsoluteTime) == 0)
                                    {
                                        //last musical symbol in bar
                                        tempADPTrack.Bars.Add(tempADPBar);
                                        tempADPBar = new ADPBar();
                                        tempADPBar.TimeSignature = timeSignature;
                                    }
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

        private int calculateBarLength(int[] _timeSignature, double _wholeNoteLength)
        {
            double beatNoteLength = _wholeNoteLength / _timeSignature[1];
            int barLength = (int)(_timeSignature[0] * beatNoteLength);
            return barLength;
        }

        private string[] convertToInputStrings(ChannelMessage _message, MidiEvent _midiEvent, double _wholeNoteLength)
        {
            if (_midiEvent.DeltaTicks == 0)
            {
                return null;
            }

            string[] resultInputStrings = new string[6];

            int[] tempDurationAndDots = calculateDurationAndDots(_midiEvent.DeltaTicks, _wholeNoteLength);

            if (_midiEvent.DeltaTicks != 0 && _message.Data2 != 0)
            {
                //rest 
                resultInputStrings[0] = "rest";
                resultInputStrings[1] = ""+tempDurationAndDots[0];
            }
            else
            {
                //note
                resultInputStrings[0] = "note";
                resultInputStrings[1] = "" + tempDurationAndDots[0];
                resultInputStrings[2] = "" + tempDurationAndDots[1];
                resultInputStrings[3] = keyValues[_message.Data1 % 12];
                if(resultInputStrings[3].Length > 1)
                {
                    resultInputStrings[4] = "" + 1;
                } else
                {
                    resultInputStrings[4] = "" + 0;
                }
                resultInputStrings[5] = ""+ (_message.Data1 / 12 - 1);
            }

            return resultInputStrings;
        }

        private int[] calculateDurationAndDots(int _deltaTime, double _wholeNoteLength)
        {
            int[] result = { 0, 0 };

            if (_deltaTime - _wholeNoteLength == 0)
            {
                result[0] = 1;
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 2) >= 0)
            {
                result[0] = 2;
                if (_deltaTime - (_wholeNoteLength / 2) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 4) >= 0)
            {
                result[0] = 4;
                if (_deltaTime - (_wholeNoteLength / 4) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 8) >= 0)
            {
                result[0] = 8;
                if (_deltaTime - (_wholeNoteLength / 8) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else if (_deltaTime - (_wholeNoteLength / 16) >= 0)
            {
                result[0] = 16;
                if (_deltaTime - (_wholeNoteLength / 16) > 0)
                {
                    result[1] = 1;
                }
                return result;
            }
            else
            {
                return result;
            }
        }

        private void InitKeyValues()
        {
            keyValues = new List<string>();
            keyValues.Add("C");
            keyValues.Add("C#");
            keyValues.Add("D");
            keyValues.Add("D#");
            keyValues.Add("E");
            keyValues.Add("F");
            keyValues.Add("F#");
            keyValues.Add("G");
            keyValues.Add("G#");
            keyValues.Add("A");
            keyValues.Add("A#");
            keyValues.Add("B");
        }
    }
}
